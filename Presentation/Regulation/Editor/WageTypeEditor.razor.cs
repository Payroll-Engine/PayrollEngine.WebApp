using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
    public partial class WageTypeEditor
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
            new(nameof(RegulationWageType.WageTypeNumber), typeof(NumericTextBox<decimal>))
            {
                Format = SystemSpecification.DecimalFormat,
                KeyField = true,
                Required = true,
                RequiredError = "Wage type number is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationWageType.Name), typeof(TextBox))
            {
                Required = true,
                RequiredError = "Name is required"
            },
            new(nameof(RegulationWageType.Description), typeof(TextBox)),
            new(nameof(RegulationWageType.ValueType), typeof(EnumListBox<ValueType>))
            {
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = "Value type is required"
            },
            new(nameof(RegulationWageType.CalendarCalculationMode), typeof(EnumListBox<CalendarCalculationMode>)),
            new(nameof(RegulationWageType.OverrideType), typeof(EnumListBox<OverrideType>)),
            new(nameof(RegulationWageType.Collectors), typeof(CollectorMultiSelect)),
            new(nameof(RegulationWageType.CollectorGroups), typeof(CsvTextBox)),
            new(nameof(RegulationWageType.Clusters), typeof(CsvTextBox)),
            new(nameof(RegulationWageType.Attributes), null),
            new(nameof(RegulationWageType.ValueExpression), typeof(TextBox))
            {
                Expression = true,
                Label = "Value script",
                Lines = 5
            },
            new(nameof(RegulationWageType.ResultExpression), typeof(TextBox))
            {
                Expression = true,
                Label = "Result script",
                Lines = 5
            }
        };
    }
}
