using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

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
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject]
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private List<RegulationField> Fields { get; set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            // common
            new(nameof(RegulationReport.Name), typeof(TextBox))
            {
                Label = Localizer.Shared.Name,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.Shared.Name),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationReport.Description), typeof(TextBox))
            {
                Label = Localizer.Shared.Description
            },
            new(nameof(RegulationReport.Category), typeof(TextBox))
            {
                Label = Localizer.Report.Category
            },
            new(nameof(RegulationReport.AttributeMode), typeof(EnumListBox<ReportAttributeMode>))
            {
                Label = Localizer.Report.AttributeMode
            },
            new(nameof(RegulationReport.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            },
            new(nameof(RegulationReport.Clusters), typeof(CsvTextBox))
            {
                Label = Localizer.Shared.Clusters
            },
            // queries
            new(nameof(RegulationReport.Queries), typeof(ReportQueryGrid))
            {
                Label = Localizer.ReportQuery.ReportQuery,
                Group = Localizer.ReportQuery.ReportQuery
            },
            // relations
            new(nameof(RegulationReport.Relations), typeof(DataRelationGrid))
            {
                Label = Localizer.Report.Relations,
                Group = Localizer.Report.Relations
            },
            // expressions and actions
            new(nameof(RegulationReport.BuildExpression), typeof(TextBox))
            {
                Label = Localizer.Report.BuildExpression,
                Expression = true,
                Lines = 8
            },
            new(nameof(RegulationReport.StartExpression), typeof(TextBox))
            {
                Label = Localizer.Report.StartExpression,
                Expression = true,
                Lines = 8
            },
            new(nameof(RegulationReport.EndExpression), typeof(TextBox))
            {
                Label = Localizer.Report.EndExpression,
                Expression = true,
                Lines = 8
            },
            // attributes
            new(nameof(RegulationReport.Attributes), null)
        };
        Fields = fields;
    }

    protected override async Task OnInitializedAsync()
    {
        SetupFields();
        await base.OnInitializedAsync();
    }
}