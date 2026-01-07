using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public partial class ItemEditorPanel
{
    [Parameter]
    public RegulationEditContext EditContext { get; set; }
    [Parameter]
    public RegulationItemType ItemType { get; set; }
    [Parameter]
    public IRegulationItem Item { get; set; }
    [Parameter]
    public EventCallback<(IRegulationItem Item, bool Modified)> StateChanged { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> SaveItem { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> DeleteItem { get; set; }
    [Parameter]
    public EventCallback<IRegulationItem> DeriveItem { get; set; }

    private bool HasItem => Item != null;
}