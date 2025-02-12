using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation.Regulation.Component;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

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
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject]
    private Localizer Localizer { get; set; }

    private List<RegulationField> Fields { get; set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            new(nameof(RegulationCaseRelation.SourceCaseName), typeof(CaseList))
            {
                Label = Localizer.CaseRelation.SourceCaseName,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.CaseRelation.SourceCaseName),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseRelation.SourceCaseSlot), typeof(CaseRelationSourceSlotList))
            {
                Label = Localizer.CaseRelation.SourceCaseSlot,
                KeyField = true,
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseRelation.TargetCaseName), typeof(CaseList))
            {
                Label = Localizer.CaseRelation.TargetCaseName,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Error.RequiredField(Localizer.CaseRelation.TargetCaseName),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseRelation.TargetCaseSlot), typeof(CaseRelationTargetSlotList))
            {
                Label = Localizer.CaseRelation.TargetCaseSlot,
                KeyField = true,
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationCaseRelation.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            },
            new(nameof(RegulationCaseRelation.Order), typeof(NumericTextBox<int>))
            {
                Label = Localizer.Shared.Order
            },
            new(nameof(RegulationCaseRelation.Clusters), typeof(CsvTextBox))
            {
                Label = Localizer.Shared.Clusters
            },
            new(nameof(RegulationCaseRelation.Attributes), null)
            {
                Label = Localizer.Attribute.Attributes
            },
            // expressions and actions
            new(nameof(RegulationCaseRelation.BuildExpression), typeof(TextBox))
            {
                Label = Localizer.CaseRelation.BuildExpression,
                ActionLabel = Localizer.CaseRelation.BuildActions,
                Expression = true,
                Action = FunctionType.CaseRelationBuild,
                Lines = 8
            },
            new(nameof(RegulationCaseRelation.ValidateExpression), typeof(TextBox))
            {
                Label = Localizer.CaseRelation.ValidateExpression,
                ActionLabel = Localizer.CaseRelation.ValidateActions,
                Expression = true,
                Action = FunctionType.CaseRelationValidate,
                Lines = 8
            }
        };
        Fields = fields;
    }

    protected override string OnValidate(RegulationCaseRelation caseRelation)
    {
        if (string.Equals(caseRelation.SourceCaseName, caseRelation.TargetCaseName))
        {
            return Localizer.CaseRelation.SourceEqualsTargetError;
        }

        return null;
    }

    protected override async Task OnInitializedAsync()
    {
        SetupFields();
        await base.OnInitializedAsync();
    }
}