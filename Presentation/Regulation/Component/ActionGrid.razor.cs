using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class ActionGrid : IDisposable
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public RegulationField Field { get; set; }
    [Parameter]
    public EventCallback<object> ValueChanged { get; set; }

    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    // ReSharper disable once UnusedAutoPropertyAccessor.Local
    private MudDataGrid<ActionItem> Grid { get; set; }
    private ItemCollection<ActionItem> Actions { get; set; }

    #region Action Commands

    private bool CanMoveActionUp(ActionItem item) =>
        Actions.IndexOf(item) > 0;

    private bool CanMoveActionDown(ActionItem item) =>
        Actions.IndexOf(item) < Actions.Count - 1;

    private async Task MoveActionUpAsync(ActionItem item)
    {
        var index = Actions.IndexOf(item);
        if (index < 1)
        {
            return;
        }

        Actions.RemoveAt(index);
        Actions.Insert(index - 1, item);

        await SetFieldValue();
    }

    private async Task MoveActionDownAsync(ActionItem action)
    {
        var index = Actions.IndexOf(action);
        if (index >= Actions.Count - 1)
        {
            return;
        }

        Actions.RemoveAt(index);
        Actions.Insert(index + 1, action);

        await SetFieldValue();
    }

    private async Task AddActionAsync()
    {
        // find next free index
        var index = 1;
        if (Actions.Any())
        {
            index = Actions.Max(x => x.Index) + 1;
        }

        // new action
        var action = new ActionItem(index);

        // position
        int? lastIndex = null;
        if (lastSelected != null)
        {
            lastIndex = Actions.IndexOf(lastSelected);
            lastSelected = null;
        }

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(ActionDialog.EditContext), EditContext },
            { nameof(ActionDialog.Item), Item },
            { nameof(ActionDialog.Field), Field },
            { nameof(ActionDialog.Action), action }
        };

        // attribute add dialog
        var dialog = await (await DialogService.ShowAsync<ActionDialog>(
            Localizer.Item.AddTitle(Localizer.Action.Action), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new attribute
        var item = dialog.Data as ActionItem;
        if (item == null)
        {
            return;
        }

        // add or insert action
        if (lastIndex.HasValue && lastIndex >= 0)
        {
            Actions.Insert(lastIndex.Value + 1, action);
        }
        else
        {
            Actions.Add(action);
        }
        await SetFieldValue();
    }

    private async Task EditActionAsync(ActionItem action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        // existing
        if (!Actions.Contains(action))
        {
            return;
        }

        // edit copy
        var editItem = new ActionItem(action);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(ActionDialog.EditContext), EditContext },
            { nameof(ActionDialog.Item), Item },
            { nameof(ActionDialog.Field), Field },
            { nameof(ActionDialog.Action), editItem }
        };

        // attribute create dialog
        var dialog = await (await DialogService.ShowAsync<ActionDialog>(
            Localizer.Item.EditTitle(Localizer.Action.Action), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new attribute
        var item = dialog.Data as ActionItem;
        if (item == null)
        {
            return;
        }

        // replace action
        Actions.Remove(action);
        Actions.Add(editItem);
        await SetFieldValue();
    }

    private async Task RemoveActionAsync(ActionItem action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        // existing
        if (!Actions.Contains(action))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.RemoveTitle(Localizer.Action.Action),
                Localizer.Item.RemoveQuery(action.Action)))
        {
            return;
        }

        // remove action
        Actions.Remove(action);
        await SetFieldValue();
    }

    private ActionItem lastSelected;

    private void SelectedActionChanged(ActionItem item) =>
        lastSelected = item;

    #endregion

    #region Value

    private string ActionFieldName => Field.GetActionFieldName();

    private List<string> FieldValue
    {
        get => Item.GetPropertyValue<List<string>>(ActionFieldName);
        set => Item.SetPropertyValue(ActionFieldName, value);
    }

    private async Task SetFieldValue()
    {
        var value = new List<string>();
        foreach (var actionItem in Actions)
        {
            value.Add(actionItem.Action);
        }

        // field value
        if (Item.IsNew() || !Field.KeyField)
        {
            var fieldValue = value;
            if (!fieldValue.Any())
            {
                var baseValue = GetBaseValue();
                if (baseValue != null && baseValue.Any() &&
                    !Field.Required && CompareTool.EqualLists(fieldValue, baseValue))
                {
                    // reset value on non-mandatory fields
                    fieldValue = null;
                }
            }

            FieldValue = fieldValue;
        }

        // notifications
        UpdateState();
        await ValueChanged.InvokeAsync(value);
    }

    private void ApplyFieldValue()
    {
        // value
        var value = FieldValue;
        if ((value == null || !value.Any()) && Field.HasBaseValues)
        {
            value = GetBaseValue();
        }
        var newValue = new ItemCollection<ActionItem>();
        if (value != null)
        {
            var index = 0;
            foreach (var action in value)
            {
                newValue.Add(new(index, action));
                index++;
            }
        }
        Actions = newValue;
    }

    private List<string> GetBaseValue() =>
        Item.GetBaseValue<List<string>>(Field.GetActionFieldName());

    #endregion

    #region Lifecycle

    private void UpdateState() =>
        StateHasChanged();

    private IRegulationItem lastObject;

    protected override async Task OnInitializedAsync()
    {
        // ensure expression/action field
        if (!Field.IsAction)
        {
            throw new PayrollException(Localizer.Error.EmptyActionField(Field.PropertyName));
        }

        lastObject = Item;
        ApplyFieldValue();
        UpdateState();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastObject != Item)
        {
            lastObject = Item;
            ApplyFieldValue();
            UpdateState();
        }
        await base.OnParametersSetAsync();
    }

    #endregion

    public void Dispose()
    {
        Actions?.Dispose();
    }
}
