using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// Regulation input
/// </summary>
internal interface IRegulationInput
{
    /// <summary>
    /// The edit context
    /// </summary>
    RegulationEditContext EditContext { get; set; }

    /// <summary>
    /// The input item
    /// </summary>
    IRegulationItem Item { get; set; }

    /// <summary>
    /// The input field
    /// </summary>
    RegulationField Field { get; set; }

    /// <summary>
    /// Value change notification event
    /// </summary>
    EventCallback<object> ValueChanged { get; set; }
}