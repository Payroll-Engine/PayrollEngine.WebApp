﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class ReportQueryGrid : IRegulationInput, IDisposable
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

    protected ItemCollection<ReportQuery> Queries { get; set; } = new();
    protected MudDataGrid<ReportQuery> Grid { get; set; }

    #region Value

    protected Dictionary<string, string> FieldValue
    {
        get => Item.GetPropertyValue<Dictionary<string, string>>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    private async Task SetFieldValue()
    {
        // field value
        var fieldValue = new Dictionary<string, string>();
        foreach (var item in Queries)
        {
            fieldValue.Add(item.Name, item.Value);
        }
        if (CompareTool.EqualDictionaries<string, string>(FieldValue, fieldValue))
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

        var queries = new List<ReportQuery>();
        if (value != null)
        {
            foreach (var query in value)
            {
                queries.Add(new()
                {
                    Name = query.Key,
                    Value = query.Value
                });
            }
        }
        Queries = new(queries);
        StateHasChanged();
    }

    #endregion

    #region Actions

    protected async Task AddQueryAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(ReportQueryDialog.Queries), FieldValue }
        };

        // report query create dialog
        var dialog = await (await DialogService.ShowAsync<ReportQueryDialog>("Add report query", parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new report query
        var item = dialog.Data as ReportQuery;
        if (item == null)
        {
            return;
        }

        // add report query
        Queries.Add(item);
        await SetFieldValue();
    }

    protected async Task EditQueryAsync(ReportQuery item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        if (!Queries.Contains(item))
        {
            return;
        }

        // edit copy
        var editItem = new ReportQuery(item);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(ReportQueryDialog.Query), editItem }
        };

        // report query edit dialog
        var dialog = await (await DialogService.ShowAsync<ReportQueryDialog>("Edit report query", parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace report query
        Queries.Remove(item);
        Queries.Add(editItem);
        await SetFieldValue();
    }

    protected async Task DeleteQueryAsync(ReportQuery item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        // existing
        if (!Queries.Contains(item))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                        "Delete report query",
                        $"Delete {item.Name} permanently?"))
        {
            return;
        }

        // remove report query
        Queries.Remove(item);
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
        Queries?.Dispose();
    }

    #endregion

}
