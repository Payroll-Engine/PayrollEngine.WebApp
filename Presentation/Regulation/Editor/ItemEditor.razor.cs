using System;
using System.Linq;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation.Component;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class ItemEditor
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public IRegulationItemValidator Validator { get; set; }
    [Parameter]
    public List<RegulationField> Fields { get; set; }
    [Parameter]
    public EventCallback<(IRegulationItem Item, bool Modified)> StateChanged { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> SaveItem { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> DeleteItem { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private IRegulationItem EditItem { get; set; }
    private MudForm form;
    private IRegulationItem lastItem;
    private bool HasItem => Item != null;
    private bool HasBaseItem => Item != null && Item.BaseItem != null;
    private bool HasId => Item.Id > 0;

    private string ItemTypeName =>
        Localizer.GroupKey(EditItem.ItemType.ToString());

    private string ItemTitle =>
        Changed ? $"{EditItem.InheritanceKey} *" : EditItem.InheritanceKey;

    private string ToUiDate(DateTime dateTime) =>
        dateTime == DateTime.MinValue ? "*" : dateTime.ToCompactString();

    #region State

    private bool Unchanged { get; set; }
    private bool Changed => !Unchanged;
    private bool Modified => HasId && Changed;

    private async Task InitStateAsync()
    {
        lastItem = Item;
        EditItem = Item?.Clone();
        await UpdateStateAsync();
    }

    private async Task UpdateStateAsync()
    {
        // new record or changed
        var unchanged = Item.Id != 0 && EditItem.Equals(Item);
        if (unchanged == Unchanged)
        {
            return;
        }
        Unchanged = unchanged;

        // notify external
        await StateChanged.InvokeAsync((Item, Modified));
    }

    #endregion

    #region Fields

    private bool HasAttributeField =>
        AttributeField != null;

    private RegulationField AttributeField =>
        Fields.FirstOrDefault(x => x.IsAttributeField);

    private List<RegulationField> GetCommonFields()
    {
        var fields = new List<RegulationField>();
        fields.AddRange(Fields.Where(x => x.KeyField));
        fields.AddRange(Fields.Where(x => !x.KeyField &&
                                          !x.Expression &&
                                          !x.IsAction &&
                                          !x.IsAttributeField &&
                                          string.IsNullOrWhiteSpace(x.Group)));
        return fields;
    }

    private List<Tuple<string, List<RegulationField>>> GetGroupFields()
    {
        var groupFields = new List<Tuple<string, List<RegulationField>>>();
        var fieldsWithGroup = Fields.Where(x => !string.IsNullOrWhiteSpace(x.Group));
        foreach (var field in fieldsWithGroup)
        {
            var groupField = groupFields.FirstOrDefault(x => string.Equals(x.Item1, field.Group));
            if (groupField == null)
            {
                groupFields.Add(new(field.Group, [field]));
            }
            else
            {
                groupField.Item2.Add(field);
            }
        }
        return groupFields;
    }

    private string GetGroupCountLabel(string groupName)
    {
        var count = Item.GetGroupCount(groupName);
        return count == 0 ? null : $" ({count})";
    }

    private string ParentTypeName =>
        EditItem.Parent != null ? Localizer.GroupKey(EditItem.Parent?.ItemType.ToString()) : null;

    private List<RegulationField> GetActionFields() =>
        Fields.Where(x => x.IsAction).ToList();

    private List<RegulationField> GetExpressionFields() =>
        Fields.Where(x => x.Expression).ToList();

    #endregion

    #region Actions

    private string ActionCountLabel => GetItemCountLabel(Item.ActionCount);
    private string ExpressionCountLabel => GetItemCountLabel(Item.ExpressionCount);
    private string AttributeCountLabel => GetItemCountLabel(Item.AttributeCount);

    private static string GetItemCountLabel(int count) =>
        count == 0 ? null : $" ({count})";

    private bool CanDelete() =>
        HasId && (EditItem.IsNew() || EditItem.IsDerived());

    protected bool CanCancel() =>
        HasId && (EditItem.IsNew() || EditItem.IsDerived());

    private bool CanSave() =>
        EditItem.IsNew() || EditItem.IsDerived();

    private bool CanDerive() =>
        EditItem.IsBase();

    private async Task OnUndoChangesAsync() =>
        await InitStateAsync();

    private async Task OnSaveItemAsync()
    {
        // validation
        if (!await form.Revalidate())
        {
            return;
        }

        // apply edit object to the referenced object
        CopyTool.CopyObjectProperties(EditItem, Item);

        // validator
        if (Validator != null)
        {
            var validation = Validator.Validate(Item);
            if (!string.IsNullOrWhiteSpace(validation))
            {
                await DialogService.ShowErrorMessageBoxAsync(
                    Localizer,
                    Localizer.Item.CreateTitle(ItemTypeName),
                    validation);
                return;
            }
        }

        // call external save action
        await SaveItem.InvokeAsync(EditItem);

        // reset state
        await InitStateAsync();
    }

    private async Task OnDeleteItemAsync()
    {
        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.DeleteTitle(ItemTypeName),
                Localizer.Item.DeleteQuery(Item.InheritanceKey)))
        {
            return;
        }
        await DeleteItem.InvokeAsync(Item);
    }

    private async Task OnDeriveItemAsync()
    {
        if (!Item.IsBase())
        {
            return;
        }
        await DeriveItem.InvokeAsync(Item);
    }

    #endregion

    #region Navigation

    private async Task HandleNavigation(LocationChangingContext context)
    {
        // without pending changes
        if (!Modified)
        {
            return;
        }

        // continue confirmation
        var result = await DialogService.ShowMessageBox(
            title: Localizer.Shared.UnsaveChangesTitle,
            message: Localizer.Shared.UnsaveChangesQuery,
            yesText: Localizer.Shared.Leave,
            cancelText: Localizer.Shared.Stay);
        if (result == null || result == false)
        {
            // block the navigation
            context.PreventNavigation();
        }
    }

    #endregion

    #region Lifecycle

    protected List<string> GetBaseRegulations()
    {
        var baseRegulations = new List<string>();
        var baseItem = Item.BaseItem;
        while (baseItem != null)
        {
            baseRegulations.Add(baseItem.RegulationName);
            baseItem = baseItem.BaseItem;
        }
        return baseRegulations;
    }

    private Dictionary<string, object> GetComponentParameters(RegulationField field)
    {
        return new()
        {
            { nameof(IRegulationInput.EditContext), EditContext },
            { nameof(IRegulationInput.Item), EditItem },
            { nameof(IRegulationInput.Field), field },
            { nameof(IRegulationInput.ValueChanged), EventCallback.Factory.Create<object>(this, UpdateStateAsync) }
        };
    }

    protected override async Task OnInitializedAsync()
    {
        await InitStateAsync();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        // object change
        if (lastItem != Item)
        {
            await InitStateAsync();
        }
        await base.OnParametersSetAsync();
    }

    #endregion

}