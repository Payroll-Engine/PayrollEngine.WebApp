using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
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
    private IConfiguration Configuration { get; set; }

    /// <summary>
    /// The root case
    /// </summary>
    protected CaseSet RootCase => Cases?.FirstOrDefault();

    protected NewCasePageBase(WorkingItems workingItems) :
           base(workingItems)
    {
    }

    #region Base type

    /// <summary>Gets the type of the case</summary>
    /// <value>The type of the case.</value>
    protected abstract CaseType CaseType { get; }

    /// <summary>
    /// Gets the name of the parent page
    /// </summary>
    /// <value>The name of the parent page</value>
    protected abstract string ParentPageName { get; }


    /// <summary>
    /// Creates a new case value provider
    /// </summary>
    /// <returns>ICaseValueProvider</returns>
    protected abstract ICaseValueProvider CaseValueProvider { get; }

    #endregion

    #region Change

    /// <summary>
    /// The change reason
    /// </summary>
    public string ChangeReason { get; set; }

    private string forecast;
    /// <summary>
    /// The forecast name
    /// </summary>
    public string Forecast
    {
        get => forecast;
        set
        {
            forecast = value;
            UpdateValidationAsync();
        }
    }

    #endregion

    #region Cases

    private ObservedHashSet<CaseSet> cases = new();
    /// <summary>
    /// The root case collection
    /// </summary>
    protected ObservedHashSet<CaseSet> Cases
    {
        get => cases;
        set
        {
            DisconnectCases();
            cases = value;
            ConnectCases();
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
                await UserNotification.ShowErrorMessageBoxAsync(
                    Localizer, "Case setup error", $"Unknown case {CaseName}");
                return;
            }

            // convert to view model
            var caseSet = new CaseSet(@case, CaseValueProvider, ValueFormatter);
            // Lookups are initially set without taking in consideration possible cases that are only shown in certain conditions
            // Those new lookups are loaded in Update Case method
            await SetupLookupsAsync(caseSet);
            await derivedCases.AddAsync(caseSet);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Case setup error", exception);
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
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Case build error", $"Unknown case {CaseName}");
                return;
            }

            // Updating cases may add more cases to existing list, therefore update of lookups is needed
            var changeCaseSet = new CaseSet(@case, CaseValueProvider, ValueFormatter);
            await CaseMerger.MergeAsync(changeCaseSet, caseSet);
            await SetupLookupsAsync(caseSet);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Case build error", exception);
        }
        finally
        {
            caseUpdating = false;
        }
    }

    /// <summary>
    /// Submit the case to the backend
    /// </summary>
    protected async Task SubmitCaseAsync()
    {
        // submit case
        var caseSet = RootCase;
        if (caseSet == null)
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Case submit error", "Missing root case to submit");
            return;
        }

        // case validation
        if (!await fieldForm.Revalidate() || !await changeForm.Revalidate())
        {
            await UserNotification.ShowErrorAsync("Case validation failed");
            return;
        }

        try
        {
            // case change setup
            var caseChangeSetup = GetCaseChange(caseSet, true);
            if (!caseChangeSetup.CollectCaseValues().Any())
            {
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Case submit error", $"Missing values for case {caseSet.Name}");
                return;
            }

            // add derived case
            var caseChange = await AddCaseAsync(caseChangeSetup);
            var issues = caseChange.GetCaseIssues();
            if (issues != null)
            {
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Case submit error", issues);
                return;
            }

            // user notification
            string message;
            var caseName = User.Language.GetLocalization(caseSet.NameLocalizations, caseSet.Name);
            if (caseChange.Values.Any())
            {
                message = $"Case {caseName} successfully submitted";
                await UserNotification.ShowMessageBoxAsync(Localizer, "Submit Case", message);
            }
            else
            {
                message = $"Ignored unchanged case {caseName}";
                await UserNotification.ShowMessageBoxAsync(Localizer, "Submit Case", message);
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
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Submit Case", exception);
        }
    }

    /// <summary>
    /// Navigate to the parent page
    /// </summary>
    protected void NavigateToParent() =>
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
                derivedCaseName, User.Id, employeeId: Employee?.Id, language: User.Language,
                caseChangeSetup: caseChangeSetup);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Build Case", exception);
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
            Case = caseSet.ToCaseChangeSetup(ValueFormatter, submitMode)
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
                        lookupNames: new[] { lookupName }, language: Session.User.Language)).FirstOrDefault();
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
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, "Lookup error", exception);
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

    public bool Valid { get; set; }

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

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await SetupCaseAsync();
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