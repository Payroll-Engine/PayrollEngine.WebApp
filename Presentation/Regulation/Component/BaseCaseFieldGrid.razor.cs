using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

public partial class BaseCaseFieldGrid : IRegulationInput, IDisposable
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
    protected IPayrollService PayrollService { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }

    protected ItemCollection<CaseFieldReference> References { get; set; } = new();

    protected List<CaseFieldReference> FieldValue
    {
        get => Item.GetPropertyValue<List<CaseFieldReference>>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

    protected MudDataGrid<CaseFieldReference> Grid { get; set; }

    protected string FieldLabel => nameof(ViewModel.Case.BaseCaseFields).ToPascalSentence();

    #region Value

    private async Task SetFieldValue()
    {
        // field value
        var fieldValue = new List<CaseFieldReference>();
        foreach (var reference in References)
        {
            fieldValue.Add(new() { Name = reference.Name, Order = reference.Order });
        }
        if (CompareTool.EqualDistinctLists(FieldValue, fieldValue))
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
        var references = FieldValue;
        References = references != null ? new(references) : new();
    }

    #endregion

    #region Actions

    protected async Task CreateReferenceAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(BaseCaseFieldsDialog.BaseCaseFields), BaseCaseFields }
        };

        // attribute create dialog
        var dialog = await (await DialogService.ShowAsync<BaseCaseFieldsDialog>("Add base case field", parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // new attribute
        var item = dialog.Data as CaseFieldReference;
        if (item == null)
        {
            return;
        }

        // add reference
        References.Add(item);
        await SetFieldValue();
    }

    protected async Task UpdateReferenceAsync(CaseFieldReference reference)
    {
        if (reference == null)
        {
            throw new ArgumentNullException(nameof(reference));
        }

        // existing
        if (!References.Contains(reference))
        {
            return;
        }

        // edit copy
        var editItem = new CaseFieldReference(reference);

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(BaseCaseFieldsDialog.BaseCaseFields), BaseCaseFields },
            { nameof(BaseCaseFieldsDialog.Reference), editItem }
        };

        // attribute edit dialog
        var dialog = await (await DialogService.ShowAsync<BaseCaseFieldsDialog>("Edit base case field", parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace reference
        References.Remove(reference);
        References.Add(editItem);
        await SetFieldValue();
    }

    protected async Task DeleteReferenceAsync(CaseFieldReference reference)
    {
        if (reference == null)
        {
            throw new ArgumentNullException(nameof(reference));
        }

        // existing
        if (!References.Contains(reference))
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowDeleteMessageBoxAsync(
                "Delete base case field",
                $"Delete {reference.Name} permanently?"))
        {
            return;
        }

        // remove reference
        References.Remove(reference);
        await SetFieldValue();
    }

    // load on demand
    private List<CaseField> baseCaseFields;
    protected List<CaseField> BaseCaseFields =>
        baseCaseFields ??= Task.Run(() => PayrollService.GetCaseFieldsAsync<CaseField>(
            new(EditContext.Tenant.Id, EditContext.Payroll.Id))).Result.OrderBy(x => x.Name).ToList();

    #endregion

    #region Lifecycle

    private IRegulationItem lastItem;

    protected override async Task OnInitializedAsync()
    {
        lastItem = Item;
        ApplyFieldValue();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (lastItem != Item)
        {
            lastItem = Item;
            ApplyFieldValue();
        }
        await base.OnParametersSetAsync();
    }

    #endregion

    public void Dispose()
    {
        References?.Dispose();
    }
}
