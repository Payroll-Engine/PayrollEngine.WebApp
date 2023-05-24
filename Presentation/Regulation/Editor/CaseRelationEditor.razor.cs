using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
    public partial class CaseRelationEditor
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
            new(nameof(RegulationCaseRelation.SourceCaseName), typeof(CaseList))
            {
                KeyField = true,
                Required = true,
                RequiredError = "Source case name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseRelation.SourceCaseSlot), typeof(CaseRelationSourceSlotList))
            {
                KeyField = true,
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseRelation.TargetCaseName), typeof(CaseList))
            {
                KeyField = true,
                Required = true,
                RequiredError = "Target case name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseRelation.TargetCaseSlot), typeof(CaseRelationTargetSlotList))
            {
                KeyField = true,
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseRelation.OverrideType), typeof(EnumListBox<OverrideType>)),
            new(nameof(RegulationCaseRelation.Order), typeof(NumericTextBox<int>)),
            new(nameof(RegulationCaseRelation.Clusters), typeof(CsvTextBox)),
            new(nameof(RegulationCaseRelation.Attributes), null),
            // expressions and actions
            new(nameof(RegulationCaseRelation.BuildExpression), typeof(TextBox))
            {
                Expression = true,
                Action = FunctionType.CaseRelationBuild,
                Label = "Build script",
                Lines = 8
            },
            new(nameof(RegulationCaseRelation.ValidateExpression), typeof(TextBox))
            {
                Expression = true,
                Action = FunctionType.CaseRelationValidate,
                Label = "Validate script",
                Lines = 8
            }
        };
    }
}
