using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
    public partial class ReportEditor
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
            // common
            new(nameof(RegulationReport.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = "Report name",
                RequiredError = "Name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationReport.Description), typeof(TextBox)),
            new(nameof(RegulationReport.Category), typeof(TextBox)),
            new(nameof(RegulationReport.AttributeMode), typeof(EnumListBox<ReportAttributeMode>)),
            new(nameof(RegulationReport.OverrideType), typeof(EnumListBox<OverrideType>)),
            new(nameof(RegulationReport.Clusters), typeof(CsvTextBox)),
            // queries
            new(nameof(RegulationReport.Queries), typeof(ReportQueryGrid))
            {
                Group = "Queries"
            },
            // relations
            new(nameof(RegulationReport.Relations), typeof(DataRelationGrid))
            {
                Group = "Relations"
            },
            // expressions and actions
            new(nameof(RegulationReport.BuildExpression), typeof(TextBox))
            {
                Expression = true,
                Label = "Build script",
                Lines = 8
            },
            new(nameof(RegulationReport.StartExpression), typeof(TextBox))
            {
                Expression = true,
                Label = "Start script",
                Lines = 8
            },
            new(nameof(RegulationReport.EndExpression), typeof(TextBox))
            {
                Expression = true,
                Label = "End script",
                Lines = 8
            },
            // attributes
            new(nameof(RegulationReport.Attributes), null)
        };
    }
}
