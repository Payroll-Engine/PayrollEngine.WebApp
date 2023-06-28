using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class ItemEditor
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public List<RegulationField> Fields { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> SaveItem { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> DeleteItem { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private Localizer Localizer { get; set; }

    private IRegulationItem EditItem { get; set; }
    private MudForm form;
    private IRegulationItem lastItem;
    protected bool HasItem => Item != null;
    protected bool HasBaseItem => Item != null && Item.BaseItem != null;

    protected bool HasId => Item.Id > 0;
    protected bool IsUnchanged { get; set; }
    protected bool IsChanged => !IsUnchanged;
    protected bool IsUnsaved => HasId && IsChanged;

    protected string ItemTypeName =>
        Localizer.FromGroupKey(EditItem.ItemType.ToString());

    protected string ItemTitle =>
        IsChanged ? $"{EditItem.InheritanceKey} *" : EditItem.InheritanceKey;

    protected string ToUiDate(DateTime dateTime) =>
        dateTime == DateTime.MinValue ? "*" : dateTime.ToCompactString();

    #region Fields

    protected bool HasAttributeField =>
        AttributeField != null;

    protected RegulationField AttributeField =>
        Fields.FirstOrDefault(x => x.IsAttributeField);

    protected List<RegulationField> GetCommonFields()
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
                groupFields.Add(new(field.Group, new List<RegulationField> { field }));
            }
            else
            {
                groupField.Item2.Add(field);
            }
        }
        return groupFields;
    }

    protected string ParentTypeName =>
        EditItem.Parent != null ? Localizer.FromGroupKey(EditItem.Parent?.ItemType.ToString()) : null;

    protected List<RegulationField> GetActionFields() =>
        Fields.Where(x => x.IsAction).ToList();

    protected List<RegulationField> GetExpressionFields() =>
        Fields.Where(x => x.Expression).ToList();

    #endregion

    #region Actions

    protected bool CanDelete() =>
        HasId && (EditItem.IsNew() || EditItem.IsDerived());

    protected bool CanCancel() =>
        HasId && (EditItem.IsNew() || EditItem.IsDerived());

    protected bool CanSave() =>
        EditItem.IsNew() || EditItem.IsDerived();

    protected bool CanDerive() =>
        EditItem.IsBase();

    protected async Task OnSaveItemAsync()
    {
        // validation
        if (!await form.Revalidate())
        {
            return;
        }

        // apply edit object to the referenced object
        CopyTool.CopyObjectProperties(EditItem, Item);

        // call external save action
        await SaveItem.InvokeAsync(EditItem);

        // reset state
        InitState();
    }

    protected async Task OnDeleteItemAsync()
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

    protected async Task OnDeriveItemAsync()
    {
        if (!Item.IsBase())
        {
            return;
        }
        await DeriveItem.InvokeAsync(Item);
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

    protected Dictionary<string, object> GetComponentParameters(RegulationField field)
    {
        return new()
        {
            { nameof(IRegulationInput.EditContext), EditContext },
            { nameof(IRegulationInput.Item), EditItem },
            { nameof(IRegulationInput.Field), field },
            { nameof(IRegulationInput.ValueChanged), EventCallback.Factory.Create<object>(this, UpdateState) }
        };
    }

    private void InitState()
    {
        lastItem = Item;
        EditItem = Item?.Clone();
        UpdateState();
    }

    private void UpdateState() =>
        IsUnchanged = EditItem.Equals(Item);

    protected override Task OnInitializedAsync()
    {
        InitState();
        return base.OnInitializedAsync();
    }

    protected override Task OnParametersSetAsync()
    {
        // object change
        if (lastItem != Item)
        {
            InitState();
        }
        return base.OnParametersSetAsync();
    }

    #endregion

}