using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
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
        public EventCallback<IRegulationItem> OverrideItem { get; set; }

        protected List<RegulationField> Fields { get; } = new() {
            new(nameof(RegulationLookupValue.Key), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = "Lookup key",
                RequiredError = "Key is required"
            },
            new(nameof(RegulationLookupValue.Value), typeof(TextBox))
            {
                Required = true,
                RequiredError = "Value is required"
            },
            new(nameof(RegulationLookupValue.RangeValue), typeof(NumericTextBox<decimal>))
            {
                Format = SystemSpecification.DecimalFormat
            },
            new(nameof(RegulationLookupValue.OverrideType), typeof(EnumListBox<OverrideType>)),
        };
    }
}
