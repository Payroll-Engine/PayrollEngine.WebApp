using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;
using TextBox = PayrollEngine.WebApp.Presentation.Regulation.Component.TextBox;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
    public partial class ReportTemplateEditor
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
            new(nameof(RegulationReportTemplate.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = "Report template name",
                RequiredError = "Name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationReportTemplate.Language), typeof(EnumListBox<Language>)),
            new(nameof(RegulationReportTemplate.Content), typeof(FileBox))
            {
                Required = true,
                RequiredError = "Content is required"
            },
            new(nameof(RegulationReportTemplate.ContentType), typeof(TextBox)),
            new(nameof(RegulationReportTemplate.Schema), typeof(FileBox)),
            new(nameof(RegulationReportTemplate.Resource), typeof(TextBox))
            {
                MaxLength = SystemSpecification.ResourceTextLength
            },
            new(nameof(RegulationReportTemplate.OverrideType), typeof(EnumListBox<OverrideType>)),
            new(nameof(RegulationReportTemplate.Attributes), null)
        };
    }
}
