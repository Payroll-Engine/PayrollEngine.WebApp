using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
    public partial class CollectorEditor
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
            new(nameof(RegulationCollector.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = "Collector name",
                RequiredError = "Name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCollector.CollectType), typeof(EnumListBox<CollectType>))
            {
                FixedBaseValue = true,
                Required = true,
                RequiredError = "Collect type is required"
            },
            new(nameof(RegulationCollector.OverrideType), typeof(EnumListBox<OverrideType>)),
            new(nameof(RegulationCollector.ValueType), typeof(EnumListBox<ValueType>)),
            new(nameof(RegulationCollector.Threshold), typeof(NumericTextBox<decimal?>))
            {
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationCollector.MinResult), typeof(NumericTextBox<decimal?>))
            {
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationCollector.MaxResult), typeof(NumericTextBox<decimal?>))
            {
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationCollector.Clusters), typeof(CsvTextBox)),
            new(nameof(RegulationCollector.Attributes), null),
            new(nameof(RegulationCollector.StartExpression), typeof(TextBox))
            {
                Expression = true,
                Label = "Start script",
                Lines = 5
            },
            new(nameof(RegulationCollector.ApplyExpression), typeof(TextBox))
            {
                Expression = true,
                Label = "Apply script",
                Lines = 5
            },
            new(nameof(RegulationCollector.EndExpression), typeof(TextBox))
            {
                Expression = true,
                Label = "End script",
                Lines = 5
            }
        };
    }
}
