using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

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
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject]
    private Localizer Localizer { get; set; }

    private List<RegulationField> Fields { get; set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            // common
            new(nameof(RegulationCase.Name), typeof(TextBox))
            {
                KeyField = true,
                Required = true,
                Label = Localizer.Shared.Name,
                RequiredError = Localizer.Error.RequiredField(Localizer.Shared.Name),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCase.Description), typeof(TextBox))
            {
                Label = Localizer.Shared.Description
            },
            new(nameof(RegulationCase.DefaultReason), typeof(TextBox))
            {
                Label = Localizer.Case.DefaultReason
            },
            new(nameof(RegulationCase.Lookups), typeof(CsvTextBox))
            {
                Label = Localizer.Lookup.Lookups
            },
            new(nameof(RegulationCase.Clusters), typeof(CsvTextBox))
            {
                Label = Localizer.Shared.Clusters
            },
            new(nameof(RegulationCase.CaseType), typeof(EnumListBox<CaseType>))
            {
                Label = Localizer.Case.CaseType,
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.Case.CaseType)
            },
            new(nameof(RegulationCase.CancellationType), typeof(EnumListBox<CaseCancellationType>))
            {
                Label = Localizer.Case.CancellationType
            },
            new(nameof(RegulationCase.Hidden), typeof(Switch))
            {
                Label = Localizer.Shared.Hidden
            },

            // derived
            new(nameof(RegulationCase.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType,
                Group = Localizer.Regulation.InheritanceDerived
            },
            new(nameof(RegulationCase.BaseCase), typeof(CaseList))
            {
                Label = Localizer.Case.BaseCase,
                Group = Localizer.Regulation.InheritanceDerived,
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCase.BaseCaseFields), typeof(BaseCaseFieldGrid))
            {
                Label = Localizer.Case.BaseCaseFields,
                Group = Localizer.Regulation.InheritanceDerived
            },

            // case slots
            new(nameof(RegulationCase.Slots), typeof(RegulationCaseSlotGrid))
            {
                Label = Localizer.Case.Slots,
                Group = Localizer.Case.Slots
            },

            // expressions and actions
            new(nameof(RegulationCase.AvailableExpression), typeof(TextBox))
            {
                Label = Localizer.Case.AvailableExpression,
                ActionLabel = Localizer.Case.AvailableActions,
                Expression = true,
                Action = FunctionType.CaseAvailable,
                Lines = 8
            },
            new(nameof(RegulationCase.BuildExpression), typeof(TextBox))
            {
                Label = Localizer.Case.BuildExpression,
                ActionLabel = Localizer.Case.BuildActions,
                Expression = true,
                Action = FunctionType.CaseBuild,
                Lines = 8
            },
            new(nameof(RegulationCase.ValidateExpression), typeof(TextBox))
            {
                Label = Localizer.Case.ValidateExpression,
                ActionLabel = Localizer.Case.ValidateActions,
                Expression = true,
                Action = FunctionType.CaseValidate,
                Lines = 8
            },

            // attributes
            new(nameof(RegulationCase.Attributes), null)
        };
        Fields = fields;
    }

    protected override async Task OnInitializedAsync()
    {
        SetupFields();
        await base.OnInitializedAsync();
    }
}