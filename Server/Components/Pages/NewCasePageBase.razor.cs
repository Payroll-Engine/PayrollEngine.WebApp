using System;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using CaseSet = PayrollEngine.WebApp.ViewModel.CaseSet;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public abstract partial class NewCasePageBase(WorkingItems workingItems) : PageBase(workingItems)
{
    private MudForm fieldForm;
    private MudForm changeForm;

    /// <summary>
    /// The working case name
    /// </summary>
    [Parameter]
    public string CaseName { get; set; }

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
    private TreeCaseSet RootCase => Cases?.FirstOrDefault();

    private string DefaultDialogTitle => Localizer.Item.CreateTitle(CaseName);

    #region Culture

    /// <summary>
    /// Case culture
    /// </summary>
    private CultureInfo CaseCulture { get; set; }

    private CultureInfo GetValueCulture(CaseField caseField) =>
        Task.Run(() => GetValueCultureAsync(caseField)).Result;

    private async Task<CultureInfo> GetValueCultureAsync(CaseField caseField) =>
        new(await GetCultureNameAsync(caseField));

    private async Task<string> GetCultureNameAsync(CaseField caseField = null)
    {
        // priority 1: case field culture
        if (caseField != null && !string.IsNullOrWhiteSpace(caseField.Culture))
        {
            return caseField.Culture;
        }

        // priority 2: employee culture
        if (Employee != null && !string.IsNullOrWhiteSpace(Employee.Culture))
        {
            return Employee.Culture;
        }

        // priority 3: division culture
        if (Payroll != null && Payroll.DivisionId > 0)
        {
            var division = await DivisionService.GetAsync<ViewModel.Division>(new(Tenant.Id), Payroll.DivisionId);
            if (division != null && !string.IsNullOrWhiteSpace(division.Culture))
            {
                return division.Culture;
            }
        }

        // priority 4: tenant culture
        if (!string.IsNullOrWhiteSpace(Tenant.Culture))
        {
            return Tenant.Culture;
        }

        // priority 5: user culture
        if (!string.IsNullOrWhiteSpace(User.Culture))
        {
            return User.Culture;
        }

        // priority 6: system culture
        return CultureInfo.CurrentCulture.Name;
    }

    /// <summary>
    /// Setup Case culture
    /// <remarks>[culture by priority]: employee > division > tenant > system</remarks>
    /// </summary>
    private async Task SetupCaseCultureAsync()
    {
        if (CaseCulture != null)
        {
            return;
        }
        var cultureName = await GetCultureNameAsync();
        CaseCulture = cultureName == null ? CultureInfo.CurrentCulture : new CultureInfo(cultureName);
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


    private string changeReason;
    /// <summary>
    /// The change reason
    /// </summary>
    private string ChangeReason
    {
        get => changeReason;
        set
        {
            changeReason = value;
            UpdateValidation();
            StateHasChanged();
        }
    }

    private bool HasForecastHistory => ForecastHistory.Any();
    private List<string> ForecastHistory { get; } = [];
    private bool ForecastSelection { get; set; }

    /// <summary>
    /// The forecast name
    /// </summary>
    private string Forecast { get; set; }

    private void OpenForecastSelection() =>
        ForecastSelection = true;

    private void CloseForecastSelection() =>
        ForecastSelection = false;

    private void SelectForecast(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            Forecast = value;
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

    /// <summary>
    /// Case info from build and validate
    /// </summary>
    private Dictionary<string, object> CaseInfo { get; set; }

    /// <summary>
    /// The root case collection
    /// </summary>
    private ObservedHashSet<TreeCaseSet> Cases
    {
        get;
        set
        {
            DisconnectCases();
            field = value;
            ConnectCases();
        }
    } = [];

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
                context: new(Tenant.Id, Payroll.Id),
                userId: User.Id,
                caseType: CaseType,
                employeeId: Employee?.Id,
                caseNames: [CaseName],
                culture: User.Culture,
                // only visible cases
                hidden: false);
            CaseAvailable = availableCases.Any();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await ShowErrorMessageBoxAsync(exception);
        }
    }

    /// <summary>
    /// Set up the case including related cases.
    /// </summary>
    private async Task SetupCaseAsync()
    {
        var derivedCases = new ObservedHashSet<TreeCaseSet>();
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
            var treeCaseSet = new TreeCaseSet(new CaseSet(@case, CaseValueProvider, ValueFormatter, TenantCulture, Localizer));
            // Lookups are initially set without taking in consideration possible cases that are only shown in certain conditions
            // Those new lookups are loaded in Update Case method
            await SetupLookupsAsync(treeCaseSet.CaseSet);
            await derivedCases.AddAsync(treeCaseSet);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await ShowErrorMessageBoxAsync(exception);
        }

        // result
        Cases = derivedCases;

        // user info
        ApplyCase(Cases.FirstOrDefault()?.CaseSet);

        // validation
        await InvokeAsync(UpdateValidation);

        // state
        StateHasChanged();
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
            var @case = await BuildCaseAsync(caseSet.Name, caseChange);
            if (@case == null)
            {
                await ShowErrorMessageBoxAsync(Localizer.Case.MissingCase(CaseName));
                return;
            }

            // Updating cases may add more cases to existing list, therefore update of lookups is needed
            var changeCaseSet = new CaseSet(@case, CaseValueProvider, ValueFormatter, TenantCulture, Localizer);

            // merge case
            await CaseMerger.MergeAsync(changeCaseSet, caseSet);

            // setup lookups
            await SetupLookupsAsync(caseSet);

            await InvokeAsync(() =>
            {
                ApplyCase(caseSet);
                StateHasChanged();
            });
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

    private void ApplyCase(CaseSet caseSet)
    {
        // case info
        var caseInfo = new Dictionary<string, object>();
        if (RootCase?.CaseSet?.Attributes != null)
        {
            foreach (var a in RootCase.CaseSet.Attributes)
            {
                LogInformation($"att: {a.Key}={a.Value}");
            }

            // edit info
            if (RootCase.CaseSet.Attributes.TryGetValue(InputAttributes.EditInfo, out var buildAttribute))
            {
                if (buildAttribute is JsonElement jsonElement)
                {
                    LogInformation($"case info: {jsonElement.ToString()}");
                    var buildInfo = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonElement.ToString());
                    foreach (var item in buildInfo)
                    {
                        caseInfo.Add(item.Key, item.Value);
                    }
                }
            }
        }
        CaseInfo = caseInfo;

        // change values
        if (caseSet != null)
        {
            // reason
            var reason = caseSet.Reason ?? caseSet.DefaultReason;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                changeReason = reason;
            }
            else if (!string.IsNullOrWhiteSpace(caseSet.DefaultReason))
            {
                changeReason = caseSet.GetLocalizedDefaultReason(PageCulture.Name);
            }

            // forecast
            if (!string.IsNullOrWhiteSpace(caseSet.Forecast))
            {
                Forecast = caseSet.Forecast;
            }
        }
    }

    /// <summary>
    /// Submit the case to the backend
    /// </summary>
    private async Task SubmitCaseAsync()
    {
        // submit case
        var caseSet = RootCase?.CaseSet;
        if (caseSet == null)
        {
            await ShowErrorMessageBoxAsync(Localizer.Case.SubmitCase,
                Localizer.Case.MissingCase(CaseName));
            return;
        }

        var caseTitle = caseSet.GetLocalizedName(PageCulture.Name);

        // case validation
        if (!await fieldForm.Revalidate() || !await changeForm.Revalidate())
        {
            await ShowErrorMessageBoxAsync(
                Localizer.Case.SubmitCaseTitle(caseTitle),
                Localizer.Case.ValidationFailed);
            return;
        }

        try
        {
            // case change setup
            var caseChangeSetup = GetCaseChange(caseSet, true);
            if (!caseChangeSetup.CollectCaseValues().Any())
            {
                await ShowErrorMessageBoxAsync(
                    Localizer.Case.SubmitCaseTitle(caseTitle),
                    Localizer.Case.MissingCase(caseTitle));
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
            if (caseChange.Values.Any())
            {
                message = Localizer.Case.CaseAdded(caseTitle);
                await UserNotification.ShowMessageBoxAsync(
                    localizer: Localizer,
                    title: Localizer.Case.SubmitCaseTitle(caseTitle),
                    message: Localizer.Case.CaseAdded(caseTitle),
                    icon: Icons.Material.Filled.Check);
            }
            else
            {
                message = Localizer.Case.CaseIgnored(caseTitle);
                await UserNotification.ShowMessageBoxAsync(
                    localizer: Localizer,
                    title: Localizer.Case.SubmitCase,
                    message: message);
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
            await ShowErrorMessageBoxAsync(
                Localizer.Case.SubmitCaseTitle(caseTitle), exception);
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
            return await PayrollService.BuildCaseAsync<Client.Model.CaseSet>(
                context: new(Tenant.Id, Payroll.Id),
                caseName: derivedCaseName,
                userId: User.Id,
                employeeId: Employee?.Id,
                culture: User.Culture,
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
        return await PayrollService.AddCaseAsync<CaseChangeSetup, ViewModel.CaseChange>(
            context: new(Tenant.Id, Payroll.Id),
            caseChangeSetup: caseChangeSetup);
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
                @case.CaseSet.FieldChanged += CaseFieldChangedHandlerAsync;
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
                @case.CaseSet.FieldChanged -= CaseFieldChangedHandlerAsync;
            }
            Cases.Added -= CaseAddedHandlerAsync;
            Cases.Removed -= CaseRemovedHandlerAsync;
        }
    }

    private Task CaseAddedHandlerAsync(object sender, TreeCaseSet caseSet)
    {
        caseSet.CaseSet.FieldChanged += CaseFieldChangedHandlerAsync;
        UpdateValidation();
        return Task.CompletedTask;
    }

    private Task CaseRemovedHandlerAsync(object sender, TreeCaseSet caseSet)
    {
        caseSet.CaseSet.FieldChanged -= CaseFieldChangedHandlerAsync;
        UpdateValidation();
        return Task.CompletedTask;
    }

    private async Task CaseFieldChangedHandlerAsync(object sender, CaseSet caseSet)
    {
        await UpdateCaseAsync(caseSet);
        UpdateValidation();
        // important note
        // this event is created by a sync property change in a view model object
        // do not call StateHasChanged() directly, otherwise the page freezes
        InvokeAsync(StateHasChanged).Wait();
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
                        context: new(Tenant.Id, Payroll.Id),
                        lookupNames: [lookupName],
                        culture: Session.User.Culture)).FirstOrDefault();
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
                                TenantCulture, lookupValue.RangeValue, valueFieldName, textFieldName));
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

    private CaseObjectValidity Validity { get; set; }

    /// <summary>
    /// Update case validation including form and build validation
    /// </summary>
    private void UpdateValidation()
    {
        // case
        CaseObjectValidity validity = null;
        if (Cases != null)
        {
            foreach (var @case in Cases)
            {
                if (!@case.CaseSet.Validity.Valid)
                {
                    validity = @case.CaseSet.Validity;
                    break;
                }
            }
        }
        Validity = validity;

        // valid state
        var valid = Validity == null && ValidCase && ValidBuild;
        if (valid != Valid)
        {
            Valid = valid;
            //StateHasChanged();
            InvokeAsync(StateHasChanged);
        }
    }

    private bool ValidCase =>
        !string.IsNullOrWhiteSpace(ChangeReason);

    private bool ValidBuild =>
        RootCase?.CaseSet.Attributes.GetValidity(PageCulture) ?? true;

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

    protected override async Task OnPageInitializedAsync()
    {
        // case culture
        await SetupCaseCultureAsync();

        // test available case
        await SetupAvailableCaseAsync();
        if (!CaseAvailable)
        {
            return;
        }

        // setup case
        await SetupCaseAsync();
        await base.OnPageInitializedAsync();
    }

    private bool initValidation;
    protected override async Task OnPageAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // forecast history
            if (HasFeature(Feature.Forecasts))
            {
                await SetupForecastHistoryAsync();
            }

            // preselect case
            if (Cases != null && Cases.Any())
            {
                Cases.First().Selected = true;
            }
        }

        // validation
        if (!initValidation && changeForm != null)
        {
            UpdateValidation();
            initValidation = true;
        }

        await base.OnPageAfterRenderAsync(firstRender);
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