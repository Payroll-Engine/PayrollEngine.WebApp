using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class LookupValueEditor
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
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private List<RegulationField> Fields { get; set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            new(nameof(RegulationLookupValue.Key), typeof(TextBox))
            {
                Label = Localizer.Shared.Key,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.Shared.Key)
            },
            new(nameof(RegulationLookupValue.Value), typeof(TextBox))
            {
                Label = Localizer.Shared.Value,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.Shared.Value)
            },
            new(nameof(RegulationLookupValue.RangeValue), typeof(NumericTextBox<decimal>))
            {
                Label = Localizer.LookupValue.RangeValue,
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationLookupValue.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            }
        };
        Fields = fields;
    }

    protected override async Task OnInitializedAsync()
    {
        SetupFields();
        await base.OnInitializedAsync();
    }
}