using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using TextBox = PayrollEngine.WebApp.Presentation.Regulation.Component.TextBox;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

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
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject]
    private Localizer Localizer { get; set; }

    private List<RegulationField> Fields { get; set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            new(nameof(RegulationReportTemplate.Name), typeof(TextBox))
            {
                Label = Localizer.Shared.Name,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.Shared.Name),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationReportTemplate.Culture), typeof(TextBox))
            {
                Label = Localizer.Shared.Culture,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.Shared.Culture),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationReportTemplate.Content), typeof(FileBox))
            {
                Label = Localizer.ReportTemplate.Content,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.ReportTemplate.Content)
            },
            new(nameof(RegulationReportTemplate.ContentType), typeof(TextBox))
            {
                Label = Localizer.ReportTemplate.ContentType,
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationReportTemplate.Schema), typeof(FileBox))
            {
                Label = Localizer.ReportTemplate.Schema
            },
            new(nameof(RegulationReportTemplate.Resource), typeof(TextBox))
            {
                Label = Localizer.ReportTemplate.Resource,
                MaxLength = SystemSpecification.ResourceTextLength
            },
            new(nameof(RegulationReportTemplate.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            },
            new(nameof(RegulationReportTemplate.Attributes), null)
        };
        Fields = fields;
    }

    protected override string OnValidate(RegulationReportTemplate template)
    {
        if (string.IsNullOrWhiteSpace(template.Content))
        {
            return Localizer.ReportTemplate.MissingContentError;
        }

        return null;
    }

    protected override async Task OnInitializedAsync()
    {
        SetupFields();
        await base.OnInitializedAsync();
    }
}