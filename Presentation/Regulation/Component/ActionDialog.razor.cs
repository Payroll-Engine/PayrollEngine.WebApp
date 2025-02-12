using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

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
    private Localizer Localizer { get; set; }

    private MudForm form;
    protected List<ActionInfo> CategoryActions { get; set; }
    private List<ActionInfo> AllActions { get; set; }

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
    protected List<ActionCategory> Categories { get; set; }

    private async Task SetupActionsAsync()
    {
        var actionInfos = await EditContext.ActionProvider.GetActions(Field.Action);
        AllActions = actionInfos.OrderBy(x => x.GetExpressionTemplate()).ToList();
    }

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
                Label = x.ToPascalSentence(),
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
        // all
        if (string.Equals(category, Localizer.Shared.All))
        {
            CategoryActions = AllActions.OrderBy(x => x.GetExpressionTemplate()).ToList();
            return;
        }

        // category
        var actionCategory = Categories.FirstOrDefault(x => string.Equals(x.Label, category));
        if (actionCategory == null)
        {
            return;
        }

        // category actions
        var categoryActions = AllActions.Where(x => x.Categories.Contains(actionCategory.Name));
        CategoryActions = categoryActions.OrderBy(x => x.GetExpressionTemplate()).ToList();
    }

    private void SelectedCategoryChanged(string chip)
    {
        SetCategory(chip);
        SelectedCategory = chip;
    }

    #endregion

    #region Editor Actions

    private void ApplyAction(ActionInfo actionInfo) =>
        // append selected expression to the action
        Action.Action += actionInfo.GetExpressionTemplate();

    private void Cancel() => MudDialog.Cancel();

    private async Task Submit()
    {
        if (await form.Revalidate())
        {
            MudDialog.Close(DialogResult.Ok(Action));
        }
    }

    #endregion

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        // ensure expression/action field
        if (!Field.IsAction)
        {
            throw new PayrollException($"Field {Field.PropertyName} has no actions.");
        }

        // actions
        await SetupActionsAsync();
        // categories
        SetupCategories();
        // default category
        SetCategory(Localizer.Shared.All);

        await base.OnInitializedAsync();
    }

    #endregion

}
