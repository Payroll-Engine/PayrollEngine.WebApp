using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Data;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class DataRelationGrid : IRegulationInput, IDisposable
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
    private Localizer Localizer { get; set; }

    protected ItemCollection<DataRelation> DataRelations { get; set; } = new();
    protected MudDataGrid<DataRelation> Grid { get; set; }

    #region Value

    protected List<DataRelation> FieldValue
    {
        get => Item.GetPropertyValue<List<DataRelation>>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    private async Task SetFieldValue()
    {
        // field value
        var fieldValue = new List<DataRelation>();
        foreach (var item in DataRelations)
        {
            fieldValue.Add(new(item));
        }
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
        // value
        var value = FieldValue;

        var dataRelations = new List<DataRelation>();
        if (value != null)
        {
            foreach (var dataRelation in value)
            {
                dataRelations.Add(new(dataRelation));
            }
        }
        DataRelations = new(dataRelations);
        StateHasChanged();
    }

    #endregion

    #region Actions

    protected async Task AddDataRelationAsync()
    {
        // data relation create dialog
        var dialog = await (await DialogService.ShowAsync<DataRelationDialog>(
            Localizer.Item.AddTitle(Localizer.Report.Relation))).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new data relation
        var item = dialog.Data as DataRelation;
        if (item == null)
        {
            return;
        }

        // add data relation
        DataRelations.Add(item);
        await SetFieldValue();
    }

    protected async Task EditDataRelationAsync(DataRelation item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        if (!DataRelations.Contains(item))
        {
            return;
        }

        // edit copy
        var editItem = new DataRelation(item);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(DataRelationDialog.DataRelation), editItem }
        };

        // data relation edit dialog
        var dialog = await (await DialogService.ShowAsync<DataRelationDialog>(
            Localizer.Item.EditTitle(Localizer.Report.Relation), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace data relation
        DataRelations.Remove(item);
        DataRelations.Add(editItem);
        await SetFieldValue();
    }

    protected async Task DeleteDataRelationAsync(DataRelation item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        if (!DataRelations.Contains(item))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                Localizer,
                Localizer.Item.DeleteTitle(Localizer.Report.Relation),
                Localizer.Item.DeleteQuery(item.Name)))
        {
            return;
        }

        // remove data relation
        DataRelations.Remove(item);
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
        DataRelations?.Dispose();
    }

    #endregion

}
