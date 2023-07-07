using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;
using CaseSet = PayrollEngine.WebApp.ViewModel.CaseSet;

namespace PayrollEngine.WebApp.Server.Pages;

public abstract partial class NewCasePageBase
{
    private MudForm fieldForm;
    private MudForm changeForm;

    /// <summary>
    /// The working case name
    /// </summary>
    [Parameter]
    public string CaseName { get; set; }

    /// <summary>
    /// Dense mode
    /// </summary>
    [Parameter]
    public bool Dense { get; set; } = true;

    [Inject]
    private IPayrollService PayrollService { get; set; }
    [Inject]
    private IDivisionService DivisionService { get; set; }
    [Inject]
    private IForecastHistoryService ForecastHistoryService { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }

    /// <summary>
    /// The root case
    /// </summary>
    private CaseSet RootCase => Cases?.FirstOrDefault();

    protected NewCasePageBase(WorkingItems workingItems) :
           base(workingItems)
    {
    }

    private string DefaultDialogTitle => Localizer.Item.AddTitle(CaseName);

    #region Culture

    private string caseCulture;
    /// <summary>
    /// Case culture
    /// <remarks>[culture by priority]: employee > division > tenant > system</remarks>
    /// </summary>
    private string CaseCulture
    {
        get
        {
            if (caseCulture != null)
            {
                return caseCulture;
            }

            // priority 1: employee culture
            if (Employee != null && !string.IsNullOrWhiteSpace(Employee.Culture))
            {
                caseCulture = Employee.Culture;
            }

            // priority 2: division culture
            if (caseCulture == null && Payroll != null && Payroll.DivisionId > 0)
            {
                var division = Task.Run(() => DivisionService.GetAsync<ViewModel.Division>(new(Tenant.Id), Payroll.DivisionId)).Result;
                if (division != null && !string.IsNullOrWhiteSpace(division.Culture))
                {
                    caseCulture = division.Culture;
                }
            }

            // priority 3: tenant culture
            if (caseCulture == null && !string.IsNullOrWhiteSpace(Tenant.Culture))
            {
                caseCulture = Tenant.Culture;
            }

            // priority 4: system culture
            return caseCulture ??= CultureInfo.CurrentCulture.Name;
        }
    }

    #endregion

    #region Derived type

    /// <summary>Gets the type of the case</summary>
    /// <value>The type of the case.</value>
    protected abstract CaseType CaseType { get; }

    /// <summary>
    /// Gets the name of the parent page
    /// </summary>
    /// <value>The name of the parent page</value>
    protected abstract string ParentPageName { get; }

    /// <summary>
    /// Gets the case value provider
    /// </summary>
    protected abstract ICaseValueProvider CaseValueProvider { get; }

    #endregion

    #region Change and Forecast

    /// <summary>
    /// The change reason
    /// </summary>
    private string ChangeReason { get; set; }

    private bool HasForecastHistory => ForecastHistory.Any();
    private List<string> ForecastHistory { get; } = new();
    private bool ForecastSelection { get; set; }

    private string forecast;
    /// <summary>
    /// The forecast name
    /// </summary>
    private string Forecast
    {
        get => forecast;
        set
        {
            forecast = value;
            UpdateValidationAsync();
        }
    }

    private void OpenForecastSelection() =>
        ForecastSelection = true;

    private void CloseForecastSelection() =>
        ForecastSelection = false;

    private void SelectForecast(object value)
    {
        var selected = value as string;
        if (!string.IsNullOrWhiteSpace(selected))
        {
            Forecast = selected;
        }
        CloseForecastSelection();
    }

    private async Task SetupForecastHistoryAsync()
    {
        var forecasts = await ForecastHistoryService.GetHistoryAsync();
        ForecastHistory.AddRange(forecasts);
    }

    #endregion

    #region Cases

    private ObservedHashSet<CaseSet> cases = new();
    /// <summary>
    /// The root case collection
    /// </summary>
    private ObservedHashSet<CaseSet> Cases
    {
        get => cases;
        set
        {
            DisconnectCases();
            cases = value;
            ConnectCases();
        }
    }

    private bool CaseAvailable { get; set; }

    /// <summary>
    /// Test for available case
    /// </summary>
    /// <returns>The case set</returns>
    private async Task SetupAvailableCaseAsync()
    {
        if (string.IsNullOrWhiteSpace(CaseName))
        {
            return;
        }
        try
        {
            var availableCases = await PayrollService.GetAvailableCasesAsync<Client.Model.CaseSet>(
                new(Tenant.Id, Payroll.Id), User.Id,
                CaseType, caseNames: new[] { CaseName }, culture: User.Culture);
            CaseAvailable = availableCases.Any();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await ShowErrorMessageBoxAsync(exception);
        }
    }


    /// <summary>
    /// Setup the case including related cases
    /// </summary>
    /// <returns></returns>
    private async Task SetupCaseAsync()
    {
        var derivedCases = new ObservedHashSet<CaseSet>();
        try
        {
            // build case
            var @case = await BuildCaseAsync(CaseName);
            if (@case == null)
            {
                await ShowErrorMessageBoxAsync(DefaultDialogTitle, Localizer.Error.UnknownItem(Localizer.Case.Case, CaseName));
                return;
            }

            // convert to view model
            var caseSet = new CaseSet(@case, CaseValueProvider, ValueFormatter, Localizer);
            // Lookups are initially set without taking in consideration possible cases that are only shown in certain conditions
            // Those new lookups are loaded in Update Case method
            await SetupLookupsAsync(caseSet);
            await derivedCases.AddAsync(caseSet);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await ShowErrorMessageBoxAsync(exception);
        }

        // result
        Cases = derivedCases;

        // selection
        if (Cases.Any())
        {
            var @case = Cases.First();
            ChangeReason = @case.DefaultReason;
            StateHasChanged();
        }
    }

    // toggle to prevent re-entry on nested case field change
    private bool caseUpdating;
    /// <summary>
    /// Update the case with the backend result
    /// </summary>
    /// <remarks>The case update will be also used by related cases</remarks>
    /// <param name="caseSet">The case set to update</param>
    private async Task UpdateCaseAsync(CaseSet caseSet)
    {
        if (caseUpdating)
        {
            return;
        }

        try
        {
            caseUpdating = true;

            // case change setup
            var caseChange = GetCaseChange(caseSet, false);

            // build case
            var @case = Task.Run(() => BuildCaseAsync(caseSet.Name, caseChange)).Result;
            if (@case == null)
            {
                await ShowErrorMessageBoxAsync(Localizer.Case.MissingCase(CaseName));
                return;
            }

            // Updating cases may add more cases to existing list, therefore update of lookups is needed
            var changeCaseSet = new CaseSet(@case, CaseValueProvider, ValueFormatter, Localizer);
            await CaseMerger.MergeAsync(changeCaseSet, caseSet);
            await SetupLookupsAsync(caseSet);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await ShowErrorMessageBoxAsync(exception);
        }
        finally
        {
            caseUpdating = false;
        }
    }

    /// <summary>
    /// Submit the case to the backend
    /// </summary>
    private async Task SubmitCaseAsync()
    {
        // submit case
        var caseSet = RootCase;
        if (caseSet == null)
        {
            await ShowErrorMessageBoxAsync(Localizer.Case.SubmitCase, Localizer.Case.MissingCase(CaseName));
            return;
        }

        // case validation
        if (!await fieldForm.Revalidate() || !await changeForm.Revalidate())
        {
            await ShowErrorMessageBoxAsync(Localizer.Case.ValidationFailed);
            return;
        }

        try
        {
            // case change setup
            var caseChangeSetup = GetCaseChange(caseSet, true);
            if (!caseChangeSetup.CollectCaseValues().Any())
            {
                await ShowErrorMessageBoxAsync(Localizer.Case.SubmitCase,
                    Localizer.Case.MissingCase(caseSet.Name));
                return;
            }

            // add derived case
            var caseChange = await AddCaseAsync(caseChangeSetup);
            var issues = caseChange.GetCaseIssues();
            if (issues != null)
            {
                await ShowErrorMessageBoxAsync(Localizer.Case.SubmitCase, issues);
                return;
            }

            // forecast history
            if (HasFeature(Feature.Forecasts) && !string.IsNullOrWhiteSpace(caseChange.Forecast))
            {
                await ForecastHistoryService.AddHistoryAsync(caseChange.Forecast);
            }

            // user notification
            string message;
            var caseName = User.Culture.GetLocalization(caseSet.NameLocalizations, caseSet.Name);
            if (caseChange.Values.Any())
            {
                message = Localizer.Case.CaseAdded(caseName);
                await UserNotification.ShowMessageBoxAsync(Localizer, Localizer.Case.SubmitCase,
                    Localizer.Case.CaseAdded(caseName));
            }
            else
            {
                message = Localizer.Case.CaseIgnored(caseName);
                await UserNotification.ShowMessageBoxAsync(Localizer, Localizer.Case.SubmitCase, message);
            }

            // log
            var appConfiguration = Configuration.GetConfiguration<AppConfiguration>();
            if (appConfiguration.LogCaseChanges)
            {
                await AddTenantLogAsync(message);
            }

            // redirect to the cases page
            NavigateTo(ParentPageName);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await ShowErrorMessageBoxAsync(Localizer.Case.SubmitCase, exception);
        }
    }

    /// <summary>
    /// Navigate to the parent page
    /// </summary>
    private void NavigateToParent() =>
        NavigateTo(ParentPageName);

    /// <summary>
    /// Build the case
    /// </summary>
    /// <param name="derivedCaseName">Name of the derived case</param>
    /// <param name="caseChangeSetup">The case change setup</param>
    /// <returns>The case set</returns>
    private async Task<Client.Model.CaseSet> BuildCaseAsync(string derivedCaseName, CaseChangeSetup caseChangeSetup = null)
    {
        try
        {
            return await PayrollService.BuildCaseAsync<Client.Model.CaseSet>(new(Tenant.Id, Payroll.Id),
                derivedCaseName, User.Id, employeeId: Employee?.Id, culture: User.Culture,
                caseChangeSetup: caseChangeSetup);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await ShowErrorMessageBoxAsync(exception);
            return null;
        }
    }

    /// <summary>
    /// Adds the case
    /// </summary>
    /// <param name="caseChangeSetup">The case change setup</param>
    /// <returns>Task&lt;CaseChange&gt;</returns>
    private async Task<ViewModel.CaseChange> AddCaseAsync(CaseChangeSetup caseChangeSetup)
    {
        if (Payroll == null)
        {
            throw new InvalidOperationException();
        }
        if (CaseType == CaseType.Employee)
        {
            if (Employee == null || !caseChangeSetup.EmployeeId.HasValue)
            {
                throw new InvalidOperationException();
            }
        }
        return await PayrollService.AddCaseAsync<CaseChangeSetup, ViewModel.CaseChange>(new(Tenant.Id, Payroll.Id), caseChangeSetup);
    }

    private CaseChangeSetup GetCaseChange(CaseSet caseSet, bool submitMode)
    {
        var caseChangeSetup = new CaseChangeSetup
        {
            DivisionId = Payroll.DivisionId,
            UserId = Session.User.Id,
            Forecast = Forecast,
            Reason = ChangeReason,
            Case = caseSet.ToCaseChangeSetup(submitMode)
        };
        if (Employee != null)
        {
            caseChangeSetup.EmployeeId = Employee.Id;
        }
        return caseChangeSetup;
    }

    private void ConnectCases()
    {
        if (Cases != null)
        {
            foreach (var @case in Cases)
            {
                @case.FieldChanged += CaseFieldChangedHandlerAsync;
            }
            Cases.Added += CaseAddedHandlerAsync;
            Cases.Removed += CaseRemovedHandlerAsync;
        }
    }

    private void DisconnectCases()
    {
        if (Cases != null)
        {
            foreach (var @case in Cases)
            {
                @case.FieldChanged -= CaseFieldChangedHandlerAsync;
            }
            Cases.Added -= CaseAddedHandlerAsync;
            Cases.Removed -= CaseRemovedHandlerAsync;
        }
    }

    private async Task CaseAddedHandlerAsync(object sender, CaseSet caseSet)
    {
        caseSet.FieldChanged += CaseFieldChangedHandlerAsync;
        await UpdateValidationAsync();
    }

    private async Task CaseRemovedHandlerAsync(object sender, CaseSet caseSet)
    {
        caseSet.FieldChanged -= CaseFieldChangedHandlerAsync;
        await UpdateValidationAsync();
    }

    private async Task CaseFieldChangedHandlerAsync(object sender, CaseSet relatedCaseSet)
    {
        await UpdateCaseAsync(relatedCaseSet);
        // await SetupCaseLookups();
        await UpdateValidationAsync();
        StateHasChanged();
    }

    #endregion

    #region Lookup

    private async Task SetupLookupsAsync(CaseSet caseSet)
    {
        try
        {
            // fields
            if (caseSet.Fields != null)
            {
                // process case fields with lookups
                var lookupFields = caseSet.Fields.Where(x => x.LookupSettings != null);
                foreach (var lookupField in lookupFields)
                {
                    // ignore update if lookups already set, switching back and forth between fields may cause to load multiple of the same lookups otherwise
                    if (lookupField.LookupValues != null && lookupField.LookupValues.Any())
                    {
                        continue;
                    }

                    // retrieve local lookup values by lookup name
                    var lookupName = lookupField.LookupSettings.LookupName;

                    // retrieve lookup data
                    LookupData lookupData = (await PayrollService.GetLookupDataAsync<LookupData>(
                        new(Tenant.Id, Payroll.Id),
                        lookupNames: new[] { lookupName }, culture: Session.User.Culture)).FirstOrDefault();
                    if (lookupData?.Values != null && lookupData.Values.Any())
                    {
                        var valueFieldName = lookupField.LookupSettings.ValueFieldName;
                        var textFieldName = lookupField.LookupSettings.TextFieldName;

                        // lookup objects
                        var lookupValues = new List<LookupObject>();
                        foreach (var lookupValue in lookupData.Values)
                        {
                            var jsonElement = JsonSerializer.Deserialize<JsonElement>(lookupValue.Value);
                            lookupValues.Add(new(jsonElement, ValueFormatter, lookupField.ValueType,
                                lookupValue.RangeValue, valueFieldName, textFieldName));
                        }

                        // apply lookup to the case field
                        if (lookupValues.Any())
                        {
                            // lookup rows
                            // sort lookups by range value
                            lookupValues.Sort((left, right) => CompareLookupValues(left.RangeValue, right.RangeValue));
                            foreach (var lookup in lookupValues)
                            {
                                lookupField.LookupValues?.Add(lookup);
                            }
                        }
                        else
                        {
                            lookupField.LookupValues?.Clear();
                        }
                    }
                }
            }

            // process lookups on related cases (recursive)
            if (caseSet.RelatedCases != null)
            {
                foreach (var relatedCase in caseSet.RelatedCases)
                {
                    await SetupLookupsAsync(relatedCase);
                }
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await ShowErrorMessageBoxAsync(exception);
        }
    }

    /// <summary>Compare two nullable decimals</summary>
    /// <param name="left">The left value to compare</param>
    /// <param name="right">The right value to compare</param>
    /// <returns>True if both values are equals</returns>
    private static int CompareLookupValues(decimal? left, decimal? right)
    {
        // both undefined
        if (left == null && right == null)
        {
            return 0;
        }
        // one side undefined
        if (left == null || right == null)
        {
            return 1;
        }

        return left.Value.CompareTo(right.Value);
    }

    #endregion

    #region Validation

    private bool Valid { get; set; }

    private Task UpdateValidationAsync()
    {
        var valid = true;
        if (Cases != null)
        {
            foreach (var @case in Cases)
            {
                if (!@case.Validity.Valid)
                {
                    valid = false;
                    break;
                }
            }
        }
        Valid = valid;
        return Task.CompletedTask;
    }

    #endregion

    #region Error

    private async Task ShowErrorMessageBoxAsync(string message) =>
        await ShowErrorMessageBoxAsync(DefaultDialogTitle, message);

    private async Task ShowErrorMessageBoxAsync(string dialogTitle, string message) =>
        await UserNotification.ShowErrorMessageBoxAsync(Localizer, dialogTitle, message);

    private async Task ShowErrorMessageBoxAsync(Exception exception) =>
        await ShowErrorMessageBoxAsync(DefaultDialogTitle, exception);

    private async Task ShowErrorMessageBoxAsync(string dialogTitle, Exception exception) =>
        await UserNotification.ShowErrorMessageBoxAsync(Localizer, dialogTitle, exception);

    #endregion

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // test available case
        await SetupAvailableCaseAsync();
        if (!CaseAvailable)
        {
            return;
        }

        // setup case
        await SetupCaseAsync();
        if (HasFeature(Feature.Forecasts))
        {
            await Task.Run(SetupForecastHistoryAsync);
        }
        await UpdateValidationAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // preselect case
            if (Cases != null && Cases.Any())
            {
                Cases.First().Selected = true;
            }
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            DisconnectCases();
        }
    }

    #endregion

}