using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
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
        public EventCallback<IRegulationItem> OverrideItem { get; set; }

        protected List<RegulationField> Fields { get; } = new() {
            new(nameof(RegulationReportParameter.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = "Report parameter name",
                RequiredError = "Name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationReportParameter.Description), typeof(TextBox)),

            new(nameof(RegulationReportParameter.Value), typeof(TextBox)),
            new(nameof(RegulationReportParameter.ValueType), typeof(EnumListBox<ValueType>))
            {
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = "Value type is required"
            },
            new(nameof(RegulationReportParameter.ParameterType), typeof(EnumListBox<ReportParameterType>)),
            new(nameof(RegulationReportParameter.OverrideType), typeof(EnumListBox<OverrideType>)),
            new(nameof(RegulationReportParameter.Attributes), null)
        };
    }
}
