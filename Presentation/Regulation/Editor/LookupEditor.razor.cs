using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class LookupEditor
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> SaveItem { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> DeleteItem { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject]
    private Localizer Localizer { get; set; }

    private List<RegulationField> Fields { get; set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            new(nameof(RegulationLookup.Name), typeof(TextBox))
            {
                Label = Localizer.Shared.Name,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Shared.Name),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationLookup.Description), typeof(TextBox))
            {
                Label = Localizer.Shared.Description
            },
            new(nameof(RegulationLookup.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            },
            new(nameof(RegulationLookup.RangeSize), typeof(NumericTextBox<decimal?>))
            {
                Label = Localizer.Lookup.RangeSize,
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationLookup.Attributes), null)
        };
        Fields = fields;
    }

    protected override Task OnInitializedAsync()
    {
        SetupFields();
        return base.OnInitializedAsync();
    }
}