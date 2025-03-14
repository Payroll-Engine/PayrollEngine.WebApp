﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class RegulationCaseSlotGrid : IRegulationInput, IDisposable
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
    private ItemCollection<CaseSlot> CaseSlots { get; } = new();

    #region Value

    private List<CaseSlot> FieldValue
    {
        get => Item.GetPropertyValue<List<CaseSlot>>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    private async Task SetFieldValue()
    {
        // field value
        var fieldValue = CaseSlots.ToList();
        if (CompareTool.EqualLists(FieldValue, fieldValue))
        {
            return;
        }
        FieldValue = fieldValue;
        ApplyFieldValue();

        // notifications
        await ValueChanged.InvokeAsync(FieldValue);
    }

    private void ApplyFieldValue()
    {
        CaseSlots.Clear();
        var fieldValue = FieldValue;
        if (fieldValue != null)
        {
            CaseSlots.AddRange(fieldValue);
        }
        StateHasChanged();
    }

    #endregion

    #region Actions

    private async Task AddCaseSlotAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(CaseSlotDialog.CaseSlots), CaseSlots }
        };

        // create dialog
        var dialog = await (await DialogService.ShowAsync<CaseSlotDialog>(
            Localizer.Item.AddTitle(Localizer.CaseSlot.CaseSlot), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new case slot
        if (dialog.Data is not CaseSlot item)
        {
            return;
        }

        // add case slot
        CaseSlots.Add(item);
        await SetFieldValue();
    }

    private async Task EditCaseSlotAsync(CaseSlot item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        var existing = CaseSlots.FirstOrDefault(x => string.Equals(x.Name, item.Name));
        if (existing == null)
        {
            return;
        }

        // edit copy
        var editItem = new CaseSlot(item);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(CaseSlotDialog.CaseSlots), CaseSlots },
            { nameof(CaseSlotDialog.CaseSlot), editItem }
        };

        // edit dialog
        var dialog = await (await DialogService.ShowAsync<CaseSlotDialog>(
            Localizer.Item.EditTitle(Localizer.CaseSlot.CaseSlot), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace case slot
        CaseSlots.Remove(item);
        CaseSlots.Add(editItem);
        await SetFieldValue();
    }

    private async Task RemoveCaseSlotAsync(CaseSlot item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        var existing = CaseSlots.FirstOrDefault(x => string.Equals(x.Name, item.Name));
        if (existing == null)
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.RemoveTitle(Localizer.CaseSlot.CaseSlot),
                Localizer.Item.RemoveQuery(item.Name)))
        {
            return;
        }

        // remove case slot
        CaseSlots.Remove(item);
        await SetFieldValue();
    }

    #endregion

    #region Lifecycle

    private IRegulationItem lastObject;

    protected override void OnInitialized()
    {
        lastObject = Item;
        ApplyFieldValue();
        base.OnInitialized();
    }

    protected override void OnParametersSet()
    {
        if (lastObject != Item)
        {
            lastObject = Item;
            ApplyFieldValue();
        }
        base.OnParametersSet();
    }

    public void Dispose()
    {
        CaseSlots?.Dispose();
    }

    #endregion

}
