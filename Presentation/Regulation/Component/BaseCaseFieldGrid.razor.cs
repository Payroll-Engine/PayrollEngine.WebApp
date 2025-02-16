using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Shared;
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
    private IPayrollService PayrollService { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private ItemCollection<CaseFieldReference> References { get; set; } = new();

    private List<CaseFieldReference> FieldValue
    {
        get => Item.GetPropertyValue<List<CaseFieldReference>>(Field.PropertyName);
        set => Item.SetPropertyValue(Field.PropertyName, value);
    }

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

    private async Task CreateReferenceAsync()
    {
        // dialog parameters
        var parameters = new DialogParameters
            {
                { nameof(BaseCaseFieldsDialog.BaseCaseFields), BaseCaseFields }
            };

        // attribute add dialog
        var dialog = await (await DialogService.ShowAsync<BaseCaseFieldsDialog>(
            Localizer.Item.AddTitle(Localizer.Case.BaseCaseField), parameters)).Result;
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

    private async Task UpdateReferenceAsync(CaseFieldReference reference)
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
        var dialog = await (await DialogService.ShowAsync<BaseCaseFieldsDialog>(
            Localizer.Item.EditTitle(Localizer.Case.BaseCaseField), parameters)).Result;
        if (dialog == null || dialog.Canceled)
        {
            return;
        }

        // replace reference
        References.Remove(reference);
        References.Add(editItem);
        await SetFieldValue();
    }

    private async Task RemoveReferenceAsync(CaseFieldReference reference)
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
                Localizer,
                Localizer.Item.RemoveTitle(Localizer.Case.BaseCaseField),
                Localizer.Item.RemoveQuery(Localizer.Case.BaseCaseField)))
        {
            return;
        }

        // remove reference
        References.Remove(reference);
        await SetFieldValue();
    }

    // load on demand
    private List<CaseField> baseCaseFields;
    private List<CaseField> BaseCaseFields
    {
        get
        {
            if (baseCaseFields != null)
            {
                return baseCaseFields;
            }

            var context = new PayrollServiceContext(EditContext.Tenant.Id, EditContext.Payroll.Id);
            baseCaseFields = Task.Run(() => PayrollService.GetCaseFieldsAsync<CaseField>(context)).Result.OrderBy(x => x.Name).ToList();
            return baseCaseFields;
        }
    }

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
