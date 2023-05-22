using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
    public partial class CaseEditor
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
            new(nameof(RegulationCase.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = "Case name",
                RequiredError = "Name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCase.Description), typeof(TextBox)),
            new(nameof(RegulationCase.DefaultReason), typeof(TextBox)),
            new(nameof(RegulationCase.Lookups), typeof(CsvTextBox)),
            new(nameof(RegulationCase.Clusters), typeof(CsvTextBox)),
            new(nameof(RegulationCase.CaseType), typeof(EnumListBox<CaseType>))
            {
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = "Case type is required"
            },
            new(nameof(RegulationCase.CancellationType), typeof(EnumListBox<CaseCancellationType>)),

            // derived
            new(nameof(RegulationCase.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Group = "Derived"
            },
            new(nameof(RegulationCase.BaseCase), typeof(CaseList))
            {
                Group = "Derived",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCase.BaseCaseFields), typeof(BaseCaseFieldGrid))
            {
                Group = "Derived"
            },

            // case slots
            new(nameof(RegulationCase.Slots), typeof(RegulationCaseSlotGrid))
            {
                Group = "Slots"
            },

            // expressions and actions
            new(nameof(RegulationCase.AvailableExpression), typeof(TextBox))
            {
                Expression = true,
                Action = FunctionType.CaseAvailable,
                Label = "Available script",
                Lines = 5
            },
            new(nameof(RegulationCase.BuildExpression), typeof(TextBox))
            {
                Expression = true,
                Action = FunctionType.CaseBuild,
                Label = "Build script",
                Lines = 5
            },
            new(nameof(RegulationCase.ValidateExpression), typeof(TextBox))
            {
                Expression = true,
                Action = FunctionType.CaseValidate,
                Label = "Validate script",
                Lines = 5
            },

            // attributes
            new(nameof(RegulationCase.Attributes), null)
        };
    }
}
