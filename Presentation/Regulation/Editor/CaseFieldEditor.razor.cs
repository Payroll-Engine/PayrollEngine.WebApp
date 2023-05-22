using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor
{
    public partial class CaseFieldEditor
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

        protected RegulationCaseField CaseField => Item as RegulationCaseField;

        protected int CaseId => CaseField.Parent.Id;

        protected List<RegulationField> Fields { get; } = new() {
            // common
            new(nameof(RegulationCaseField.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = "Case field name",
                RequiredError = "Name is required",
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseField.Description), typeof(TextBox)),
            new(nameof(RegulationCaseField.CancellationMode), typeof(EnumListBox<CaseFieldCancellationMode>)),
            new(nameof(RegulationCaseField.Order), typeof(NumericTextBox<int>)),
            new(nameof(RegulationCaseField.Tags), typeof(CsvTextBox)),
            new(nameof(RegulationCaseField.Clusters), typeof(CsvTextBox)),
            new(nameof(RegulationCaseField.OverrideType), typeof(EnumListBox<OverrideType>)),

            // time
            new(nameof(RegulationCaseField.TimeType), typeof(EnumListBox<CaseFieldTimeType>))
            {
                Group = "Time",
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = "Time type is required"
            },
            new(nameof(RegulationCaseField.TimeUnit), typeof(EnumListBox<CaseFieldTimeUnit>))
            {
                Group = "Time",
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = "Time unit is required"
            },
            new(nameof(RegulationCaseField.StartDateType), typeof(EnumListBox<CaseFieldDateType>))
            {
                Group = "Time"
            },
            new(nameof(RegulationCaseField.DefaultStart), typeof(TextBox))
            {
                Group = "Time"
            },
            new(nameof(RegulationCaseField.EndDateType), typeof(EnumListBox<CaseFieldDateType>))
            {
                Group = "Time"
            },
            new(nameof(RegulationCaseField.DefaultEnd), typeof(TextBox))
            {
                Group = "Time"
            },
            new(nameof(RegulationCaseField.EndMandatory), typeof(Switch))
            {
                Group = "Time"
            },

            // value
            new(nameof(RegulationCaseField.ValueType), typeof(EnumListBox<ValueType>))
            {
                Group = "Value",
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = "Value type is required"
            },
            new(nameof(RegulationCaseField.ValueScope), typeof(EnumListBox<ValueScope>))
            {
                Group = "Value",
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = "Value scope is required"
            },
            new(nameof(RegulationCaseField.ValueCreationMode), typeof(EnumListBox<CaseValueCreationMode>))
            {
                Group = "Value"
            },
            new(nameof(RegulationCaseField.DefaultValue), typeof(TextBox))
            {
                Group = "Value"
            },
            new(nameof(RegulationCaseField.ValueMandatory), typeof(Switch))
            {
                Group = "Value"
            },
            new(nameof(RegulationCaseField.LookupSettings), typeof(LookupSettingsBox))
            {
                Group = "Value"
            },

            // actions
            new(nameof(RegulationCaseField.BuildActions), typeof(TextBox))
            {
                Action = FunctionType.CaseBuild,
                Lines = 5
            },
            new(nameof(RegulationCaseField.ValidateActions), typeof(TextBox))
            {
                Action = FunctionType.CaseValidate,
                Lines = 5
            },

            // attributes
            new(nameof(RegulationCaseField.Attributes), null)
        };
    }
}
