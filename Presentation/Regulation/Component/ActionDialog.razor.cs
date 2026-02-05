using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Action;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class ActionDialog
{
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }

    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public ActionItem Action { get; set; }
    [Parameter]
    public EventCallback ActionAccepted { get; set; }
    [Parameter]
    public EventCallback ActionCanceled { get; set; }

    [Inject]
    private ITenantService TenantService { get; set; }
    [Inject]
    private IPayrollService PayrollService { get; set; }
    [Inject]
    private IUserNotificationService UserNotification { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private enum ActionWorkState
    {
        Hidden,
        Loading,
        Visible
    }

    private MudForm form;
    private MudTextField<string> actionEdit;
    private List<ActionInfo> CategoryActions { get; set; }
    private List<ActionInfo> AllActions { get; set; }
    private Localizer Localizer => LocalizerService.Localizer;

    private bool SupportedMarker(MarkerType markerType) =>
        markerType.SupportedFunction(Field.Action);

    #region Action Category

    private static readonly Dictionary<string, int> CategoryOrders = new()
    {
        { "Field", 1 },
        { "FieldInput", 2 },
        { "FieldStart", 2 },
        { "FieldEnd", 3 },
        { "FieldPeriod", 3 },
        { "FieldValue", 4 },
        { "Compare", 5 },
        { "Validation", 6 },
        { "Tool", 7 }
    };
    private static readonly int UnknownCategoryOrder = 8;

    protected string SelectedCategory { get; set; }
    private List<ActionCategory> Categories { get; set; }

    private ActionWorkState ActionState { get; set; }
    private async Task ToggleActionsAsync()
    {
        if (AllActions == null)
        {
            // actions
            if (await SetupActionsAsync())
            {
                // categories
                SetupCategories();
                // default category
                SetCategory(Localizer.Shared.All);
            }
        }

        var newState = ActionState;
        switch (ActionState)
        {
            case ActionWorkState.Hidden:
            case ActionWorkState.Loading:
                newState = ActionWorkState.Visible;
                break;
            case ActionWorkState.Visible:
                newState = ActionWorkState.Hidden;
                break;
        }
        if (newState == ActionState)
        {
            return;
        }

        // state change
        ActionState = newState;
        StateHasChanged();
    }

    private async Task<bool> SetupActionsAsync()
    {
        try
        {
            // loading state
            ActionState = ActionWorkState.Loading;
            StateHasChanged();

            var actionInfos = await EditContext.ActionProvider.GetActions(Field.Action);
            AllActions = actionInfos.OrderBy(x => x.GetExpressionTemplate()).ToList();
            return true;
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Document.Documents, exception);
        }
        return false;
    }

    #endregion

    #region Category

    private void SetupCategories()
    {
        if (AllActions == null)
        {
            return;
        }

        // category names
        var categoryNames = new HashSet<string>();
        foreach (var action in AllActions)
        {
            if (action.Categories != null)
            {
                foreach (var category in action.Categories)
                {
                    categoryNames.Add(category);
                }
            }
        }

        // action categories
        var categories = new List<ActionCategory>();
        if (categoryNames.Any())
        {
            categories.AddRange(categoryNames.Select(x => new ActionCategory
            {
                Name = x,
                Label = x,
                DisplayOrder = CategoryOrders.GetValueOrDefault(x, UnknownCategoryOrder)
            }));
        }

        // category order
        var orderedCategories = categories.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Label).ToList();

        // system category 'all'
        orderedCategories.Insert(0, new()
        {
            Name = null,
            Label = Localizer.Shared.All
        });

        Categories = orderedCategories;
    }

    private void SetCategory(string category)
    {
        // category
        if (AllActions == null || Categories == null)
        {
            CategoryActions = [];
            return;
        }

        // all
        if (string.Equals(category, Localizer.Shared.All))
        {
            CategoryActions = AllActions.OrderBy(x => x.GetExpressionTemplate()).ToList();
            return;
        }

        var actionCategory = Categories.FirstOrDefault(x => string.Equals(x.Label, category));
        if (actionCategory == null)
        {
            return;
        }

        // category actions
        var categoryActions = AllActions.Where(x => x.Categories != null && x.Categories.Contains(actionCategory.Name));
        CategoryActions = categoryActions.OrderBy(x => x.GetExpressionTemplate()).ToList();
    }

    private void SelectedCategoryChanged(string chip)
    {
        SetCategory(chip);
        SelectedCategory = chip;
    }

    #endregion

    #region Action Edit

    private void FocusActionEdit() => actionEdit.FocusAsync();

    private void AppendSyntax(string syntax, bool preSpace)
    {
        if (string.IsNullOrWhiteSpace(syntax))
        {
            return;
        }

        if (preSpace && !string.IsNullOrWhiteSpace(Action.Action) &&
            !Action.Action.EndsWith(' ') && !syntax.StartsWith(' '))
        {
            syntax = ' ' + syntax;
        }

        // append selected token to the action
        Action.Action += syntax;

        // set focus
        FocusActionEdit();
    }

    #endregion

    #region Action Marker

    private bool AvailableMarker(MarkerType markerType)
    {
        if (!SupportedMarker(markerType))
        {
            return false;
        }
        switch (markerType)
        {
            case MarkerType.Condition:
                // empty action only
                return string.IsNullOrWhiteSpace(Action.Action);
            case MarkerType.ConditionTrue:
                // condition required and no duplicates allowed
                return !string.IsNullOrWhiteSpace(Action.Action) &&
                       Action.Action.StartsWith(MarkerType.Condition.GetSyntax()) &&
                       !Action.Action.Contains(markerType.GetSyntax());
            case MarkerType.ConditionFalse:
                // condition-true required and no duplicates allowed
                return !string.IsNullOrWhiteSpace(Action.Action) &&
                       Action.Action.Contains(MarkerType.ConditionTrue.GetSyntax()) &&
                       !Action.Action.Contains(markerType.GetSyntax());
        }

        return true;
    }

    private void AppendMarker(MarkerType markerType) =>
        AppendSyntax(GetMarkerSyntax(markerType), preSpace: markerType != MarkerType.Condition);

    private void AppendMarker(MarkerType markerType, string text)
    {
        var syntax = GetMarkerSyntax(markerType);
        if (string.IsNullOrWhiteSpace(syntax))
        {
            return;
        }
        AppendSyntax(syntax + text, preSpace: true);
    }

    private string GetMarkerSyntax(MarkerType markerType)
    {
        if (!AvailableMarker(markerType))
        {
            return null;
        }
        var syntax = string.Empty;

        // pre code: space to previous syntax
        if (markerType != MarkerType.Condition &&
            !string.IsNullOrWhiteSpace(Action.Action) &&
            !Action.Action.EndsWith(' '))
        {
            syntax = " ";
        }

        // main code
        syntax += markerType.GetSyntax();

        // post code: condition separator
        switch (markerType)
        {
            case MarkerType.Condition:
            case MarkerType.ConditionTrue:
            case MarkerType.ConditionFalse:
                syntax += ' ';
                break;
        }

        return syntax;
    }

    #endregion

    #region Properties

    private List<ActionInfo> Properties { get; } = [];
    private async Task LoadPropertiesAsync()
    {
        // load action properties
        var properties = await TenantService.GetSystemScriptActionPropertiesAsync<ActionInfo>(
            tenantId: EditContext.Tenant.Id,
            functionType: Field.Action);
        Properties.AddRange(properties);
    }

    private string GetPropertyType(ActionInfo property)
    {
        var type = property.Categories.FirstOrDefault()?.ToLower();
        if (string.Equals(type, nameof(DateTime), StringComparison.InvariantCultureIgnoreCase))
        {
            return "date";
        }
        if (string.Equals(type, nameof(Int32), StringComparison.InvariantCultureIgnoreCase))
        {
            return "int";
        }
        if (string.Equals(type, nameof(Boolean), StringComparison.InvariantCultureIgnoreCase))
        {
            return "bool";
        }
        return type;
    }

    #endregion

    #region Lookups

    private List<string> Lookups { get; } = [];
    private async Task LoadLookupsAsync()
    {
        if (!SupportedMarker(MarkerType.LookupValue))
        {
            return;
        }

        // load derived payroll lookups
        var context = new PayrollServiceContext(EditContext.Tenant.Id, EditContext.Payroll.Id);
        var lookups = (await PayrollService.GetLookupsAsync<Lookup>(context)).Select(GetRefName).ToList();
        Lookups.AddRange(lookups);
    }

    private void AppendLookup(string name) =>
        AppendMarker(MarkerType.LookupValue, name + '(');

    #endregion

    #region Case Fields

    private List<string> CaseFields { get; } = [];
    private async Task LoadCaseFieldsAsync()
    {
        if (!SupportedMarker(MarkerType.CaseField) && !SupportedMarker(MarkerType.CaseValue))
        {
            return;
        }

        // load derived payroll case fields
        var context = new PayrollServiceContext(EditContext.Tenant.Id, EditContext.Payroll.Id);
        var caseFields = (await PayrollService.GetCaseFieldsAsync<CaseField>(context)).Select(GetRefName).ToList();
        CaseFields.AddRange(caseFields);
    }

    private void AppendCaseField(string name) =>
        AppendMarker(MarkerType.CaseField, name);

    private void AppendCaseValue(string name) =>
        AppendMarker(MarkerType.CaseValue, name);

    private void AppendProperty(string name) =>
        AppendSyntax(name, preSpace: false);

    #endregion

    #region Collectors

    private List<string> Collectors { get; } = [];
    private async Task LoadCollectorsAsync()
    {
        if (!SupportedMarker(MarkerType.Collector))
        {
            return;
        }

        // load derived payroll collectors
        var context = new PayrollServiceContext(EditContext.Tenant.Id, EditContext.Payroll.Id);
        var collectors = (await PayrollService.GetCollectorsAsync<Collector>(context)).Select(GetRefName).ToList();
        Collectors.AddRange(collectors);
    }

    private void AppendCollector(string name) =>
        AppendMarker(MarkerType.Collector, name);

    #endregion

    #region Wage Types

    private List<string> WageTypes { get; } = [];
    private async Task LoadWageTypesAsync()
    {
        if (!SupportedMarker(MarkerType.WageType))
        {
            return;
        }

        // load derived payroll wage types
        var context = new PayrollServiceContext(EditContext.Tenant.Id, EditContext.Payroll.Id);
        var wageTypes = (await PayrollService.GetWageTypesAsync<WageType>(context)).Select(GetRefName).ToList();
        WageTypes.AddRange(wageTypes);
    }

    private void AppendWageType(string name) =>
        AppendMarker(MarkerType.WageType, name);

    #endregion

    #region Editor Actions

    private void AppendAction(ActionInfo actionInfo)
    {
        var syntax = actionInfo.GetExpressionTemplate();
        var paramIndex = syntax.IndexOf('(');
        if (paramIndex > 0)
        {
            syntax = syntax.Substring(0, paramIndex + 1);
        }
        AppendSyntax(syntax, preSpace: true);
    }

    #endregion

    #region Lifecycle

    private string GetRefName(INameObject nameObject)
    {
        // remove namespace
        var name = nameObject.Name;
        var index = name.LastIndexOf('.');
        return index > 0 ? name.Substring(index + 1) : name;
    }

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        if (await form.Revalidate())
        {
            MudDialog.Close(DialogResult.Ok(Action));
        }
    }

    protected override async Task OnInitializedAsync()
    {
        // ensure expression/action field
        if (!Field.IsAction)
        {
            throw new PayrollException($"Field {Field.PropertyName} has no actions.");
        }

        await LoadPropertiesAsync();
        await LoadLookupsAsync();
        await LoadCaseFieldsAsync();
        await LoadCollectorsAsync();
        await LoadWageTypesAsync();

        await base.OnInitializedAsync();
    }

    #endregion

}
