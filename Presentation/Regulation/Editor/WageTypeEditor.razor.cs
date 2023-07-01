using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

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
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    [Inject]
    private Localizer Localizer { get; set; }

    private List<RegulationField> Fields { get; set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            new(nameof(RegulationWageType.WageTypeNumber), typeof(NumericTextBox<decimal>))
            {
                Label = Localizer.WageType.WageTypeNumber,
                Format = SystemSpecification.DecimalFormat,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.WageType.WageTypeNumber),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationWageType.Name), typeof(TextBox))
            {
                Label = Localizer.Shared.Name,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Shared.Name)
            },
            new(nameof(RegulationWageType.Description), typeof(TextBox))
            {
                Label = Localizer.Shared.Description,
                Expression = true,
                Lines = 8
            },
            new(nameof(RegulationWageType.ValueType), typeof(EnumListBox<ValueType>))
            {
                Label = Localizer.Shared.ValueType,
                FixedBaseValue = true,
                ReadOnly = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Shared.ValueType)
            },
            new(nameof(RegulationWageType.Calendar), typeof(TextBox))
            {
                Label = Localizer.Calendar.Calendar,
                Help = Localizer.WageType.CalendarHelp
            },
            new(nameof(RegulationWageType.OverrideType), typeof(EnumListBox<OverrideType>))
            {
                Label = Localizer.Shared.OverrideType
            },
            new(nameof(RegulationWageType.Collectors), typeof(CollectorMultiSelect))
            {
                Label = Localizer.Collector.Collectors
            },
            new(nameof(RegulationWageType.CollectorGroups), typeof(CsvTextBox))
            {
                Label = Localizer.WageType.CollectorGroups
            },
            new(nameof(RegulationWageType.Clusters), typeof(CsvTextBox))
            {
                Label = Localizer.Shared.Clusters
            },
            new(nameof(RegulationWageType.Attributes), null)
            {
                Label = Localizer.Attribute.Attributes
            },
            new(nameof(RegulationWageType.ValueExpression), typeof(TextBox))
            {
                Label = Localizer.WageType.ValueExpression,
                Expression = true,
                Lines = 8
            },
            new(nameof(RegulationWageType.ResultExpression), typeof(TextBox))
            {
                Label = Localizer.WageType.ResultExpression,
                Expression = true,
                Lines = 8
            }
        };
        Fields = fields;
    }

    protected override Task OnInitializedAsync()
    {
        SetupFields();
        return base.OnInitializedAsync();
    }
}