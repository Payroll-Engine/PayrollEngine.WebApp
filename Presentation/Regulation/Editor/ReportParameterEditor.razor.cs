using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class ReportParameterEditor
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

    protected List<RegulationField> Fields { get; private set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            new(nameof(RegulationReportParameter.Name), typeof(TextBox))
            {
                Label = Localizer.Shared.Name,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Shared.Name),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationReportParameter.Description), typeof(TextBox))
            {
                Label = Localizer.Shared.Description
            },
            new(nameof(RegulationReportParameter.Value), typeof(TextBox))
            {
                Label = Localizer.Shared.Value
            },
            new(nameof(RegulationReportParameter.ValueType), typeof(EnumListBox<ValueType>))
            {
                Label = Localizer.Shared.ValueType,
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Shared.ValueType)
            },
            new(nameof(RegulationReportParameter.ParameterType), typeof(EnumListBox<ReportParameterType>))
            {
                Label = Localizer.ReportParameter.ParameterType
            },
            new(nameof(RegulationReportParameter.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            },
            new(nameof(RegulationReportParameter.Attributes), null)
        };
        Fields = fields;
    }

    protected override Task OnInitializedAsync()
    {
        SetupFields();
        return base.OnInitializedAsync();
    }
}