using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation.Regulation.Component;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class CollectorEditor
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter] 
    public EventCallback<(IRegulationItem Item, bool Modified)> StateChanged { get; set; }
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
            new(nameof(RegulationCollector.Name), typeof(TextBox))
            {
                Label = Localizer.Shared.Name,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.Shared.Name),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCollector.CollectMode), typeof(EnumListBox<CollectMode>))
            {
                Label = Localizer.Collector.CollectMode,
                FixedBaseValue = true,
                Required = true,
                RequiredError = "Collect type is required"
            },
            new(nameof(RegulationCollector.Negated), typeof(Switch))
            {
                Label = Localizer.Collector.Negated
            },
            new(nameof(RegulationCollector.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            },
            new(nameof(RegulationCollector.ValueType), typeof(EnumListBox<ValueType>))
            {
                Label = Localizer.Shared.ValueType
            },
            new(nameof(RegulationCollector.Culture), typeof(TextBox))
            {
                Label = Localizer.Shared.Culture
            },
            new(nameof(RegulationCollector.Threshold), typeof(NumericTextBox<decimal?>))
            {
                Label = Localizer.Collector.Threshold,
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationCollector.MinResult), typeof(NumericTextBox<decimal?>))
            {
                Label = Localizer.Collector.MinResult,
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationCollector.MaxResult), typeof(NumericTextBox<decimal?>))
            {
                Label = Localizer.Collector.MaxResult,
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationCollector.Clusters), typeof(CsvTextBox))
            {
                Label = Localizer.Shared.Clusters
            },
            new(nameof(RegulationCollector.Attributes), null)
            {
                Label = Localizer.Attribute.Attributes
            },
            new(nameof(RegulationCollector.StartExpression), typeof(TextBox))
            {
                Label = Localizer.Collector.StartExpression,
                ActionLabel = Localizer.Collector.StartActions,
                Expression = true,
                Action = FunctionType.CollectorStart
            },
            new(nameof(RegulationCollector.ApplyExpression), typeof(TextBox))
            {
                Label = Localizer.Collector.ApplyExpression,
                ActionLabel = Localizer.Collector.ApplyActions,
                Expression = true,
                Action = FunctionType.CollectorApply
            },
            new(nameof(RegulationCollector.EndExpression), typeof(TextBox))
            {
                Label = Localizer.Collector.EndExpression,
                ActionLabel = Localizer.Collector.EndActions,
                Expression = true,
                Action = FunctionType.CollectorEnd
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