using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class CaseFieldEditor
{
    [Parameter] public RegulationEditContext EditContext { get; set; }
    [Parameter] public IRegulationItem Item { get; set; }
    [Parameter] public EventCallback<IRegulationItem> SaveItem { get; set; }
    [Parameter] public EventCallback<IRegulationItem> DeleteItem { get; set; }
    [Parameter] public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject] private Localizer Localizer { get; set; }

    private RegulationCaseField CaseField => Item as RegulationCaseField;

    protected int CaseId => CaseField.Parent.Id;

    private List<RegulationField> Fields { get; set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            // common
            new(nameof(RegulationCaseField.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = Localizer.Shared.Name,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Shared.Name),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseField.Description), typeof(TextBox))
            {
                Label = Localizer.Shared.Description
            },
            new(nameof(RegulationCaseField.Culture), typeof(TextBox))
            {
                Label = Localizer.Shared.Culture,
                Help = Localizer.CaseField.CultureHelp
            },
            new(nameof(RegulationCaseField.CancellationMode), typeof(EnumListBox<CaseFieldCancellationMode>))
            {
                Label = Localizer.CaseField.CancellationMode
            },
            new(nameof(RegulationCaseField.Order), typeof(NumericTextBox<int>))
            {
                Label = Localizer.Shared.Order
            },
            new(nameof(RegulationCaseField.Tags), typeof(CsvTextBox))
            {
                Label = Localizer.Shared.Tags
            },
            new(nameof(RegulationCaseField.Clusters), typeof(CsvTextBox))
            {
                Label = Localizer.Shared.Clusters
            },
            new(nameof(RegulationCaseField.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            },

            // time
            new(nameof(RegulationCaseField.TimeType), typeof(EnumListBox<CaseFieldTimeType>))
            {
                Label = Localizer.CaseField.TimeType,
                Group = Localizer.CaseField.TimeGroup,
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.CaseField.TimeType)
            },
            new(nameof(RegulationCaseField.TimeUnit), typeof(EnumListBox<CaseFieldTimeUnit>))
            {
                Label = Localizer.CaseField.TimeUnit,
                Group = Localizer.CaseField.TimeGroup,
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.CaseField.TimeUnit)
            },
            new(nameof(RegulationCaseField.StartDateType), typeof(EnumListBox<CaseFieldDateType>))
            {
                Label = Localizer.CaseField.StartDateType,
                Group = Localizer.CaseField.TimeGroup
            },
            new(nameof(RegulationCaseField.DefaultStart), typeof(TextBox))
            {
                Label = Localizer.CaseField.DefaultStart,
                Group = Localizer.CaseField.TimeGroup
            },
            new(nameof(RegulationCaseField.EndDateType), typeof(EnumListBox<CaseFieldDateType>))
            {
                Label = Localizer.CaseField.EndDateType,
                Group = Localizer.CaseField.TimeGroup
            },
            new(nameof(RegulationCaseField.DefaultEnd), typeof(TextBox))
            {
                Label = Localizer.CaseField.DefaultEnd,
                Group = Localizer.CaseField.TimeGroup
            },
            new(nameof(RegulationCaseField.EndMandatory), typeof(Switch))
            {
                Label = Localizer.CaseField.EndMandatory,
                Group = Localizer.CaseField.TimeGroup
            },

            // value
            new(nameof(RegulationCaseField.ValueType), typeof(EnumListBox<ValueType>))
            {
                Label = Localizer.Shared.ValueType,
                Group = Localizer.Shared.Value,
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Shared.ValueType)
            },
            new(nameof(RegulationCaseField.ValueScope), typeof(EnumListBox<ValueScope>))
            {
                Label = Localizer.CaseField.ValueScope,
                Group = Localizer.Shared.Value,
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.CaseField.ValueScope)
            },
            new(nameof(RegulationCaseField.ValueCreationMode), typeof(EnumListBox<CaseValueCreationMode>))
            {
                Label = Localizer.CaseField.ValueCreationMode,
                Group = Localizer.Shared.Value
            },
            new(nameof(RegulationCaseField.DefaultValue), typeof(TextBox))
            {
                Label = Localizer.CaseField.DefaultValue,
                Group = Localizer.Shared.Value
            },
            new(nameof(RegulationCaseField.ValueMandatory), typeof(Switch))
            {
                Label = Localizer.CaseField.ValueMandatory,
                Group = Localizer.Shared.Value
            },
            new(nameof(RegulationCaseField.LookupSettings), typeof(LookupSettingsBox))
            {
                Label = Localizer.CaseField.LookupSettings,
                Group = Localizer.Shared.Value
            },

            // actions
            new(nameof(RegulationCaseField.BuildActions), typeof(TextBox))
            {
                Label = Localizer.CaseField.BuildActions,
                ActionLabel = Localizer.CaseField.BuildActions,
                Action = FunctionType.CaseBuild,
                Lines = 8
            },
            new(nameof(RegulationCaseField.ValidateActions), typeof(TextBox))
            {
                Label = Localizer.CaseField.ValidateActions,
                ActionLabel = Localizer.CaseField.ValidateActions,
                Action = FunctionType.CaseValidate,
                Lines = 8
            },

            // attributes
            new(nameof(RegulationCaseField.Attributes), null)
        };
        Fields = fields;
    }

    protected override Task OnInitializedAsync()
    {
        SetupFields();
        return base.OnInitializedAsync();
    }
}