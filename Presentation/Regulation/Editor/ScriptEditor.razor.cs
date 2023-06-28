using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation.Regulation.Component;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class ScriptEditor
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

    protected List<RegulationField> Fields { get; private set; }

    private void SetupFields()
    {
        var fields = new List<RegulationField>
        {
            new(nameof(RegulationScript.Name), typeof(TextBox))
            {
                Label = Localizer.Shared.Name,
                KeyField = true,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Shared.Name),
                MaxLength = SystemSpecification.KeyTextLength
            },
            new(nameof(RegulationScript.Value), typeof(TextBox))
            {
                Label = Localizer.Script.Script,
                Expression = true,
                Lines = 25,
                Required = true,
                RequiredError = Localizer.Shared.RequiredField(Localizer.Script.Script)
            },
            new(nameof(RegulationScript.FunctionTypes), typeof(MultiEnumListBox<FunctionType>))
            {
                Label = Localizer.Script.FunctionTypes
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