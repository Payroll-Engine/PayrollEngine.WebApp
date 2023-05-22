﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
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

    protected ItemCollection<CaseSlot> CaseSlots { get; } = new();
    protected MudDataGrid<CaseSlot> Grid { get; set; }

    #region Value

    protected List<CaseSlot> FieldValue
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
        CaseSlots.AddRange(FieldValue);
        StateHasChanged();
    }

    #endregion

    #region Actions

    protected async Task AddCaseSlotAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(CaseSlotDialog.CaseSlots), CaseSlots }
        };

        // create dialog
        var dialog = await (await DialogService.ShowAsync<CaseSlotDialog>("Add case slot", parameters)).Result;
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

    protected async Task EditCaseSlotAsync(CaseSlot item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        var existing = CaseSlots.FirstOrDefault(x => string.Equals(x.Name, item.Name));
        if (existing != null)
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
        var dialog = await (await DialogService.ShowAsync<CaseSlotDialog>("Edit case slot", parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace case slot
        CaseSlots.Remove(item);
        CaseSlots.Add(editItem);
        await SetFieldValue();
    }

    protected async Task DeleteCaseSlotAsync(CaseSlot item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        var existing = CaseSlots.FirstOrDefault(x => string.Equals(x.Name, item.Name));
        if (existing != null)
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                        "Delete case slot",
                        $"Delete {item.Name} permanently?"))
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

    protected override async Task OnInitializedAsync()
    {
        lastObject = Item;
        ApplyFieldValue();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastObject != Item)
        {
            lastObject = Item;
            ApplyFieldValue();
        }
        await base.OnParametersSetAsync();
    }

    public void Dispose()
    {
        CaseSlots?.Dispose();
    }

    #endregion

}
