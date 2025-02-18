namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Regulation item validator
/// </summary>
public interface IRegulationItemValidator
{
    /// <summary>
    /// Validate item
    /// </summary>
    /// <param name="item">Item to validate</param>
    /// <returns>Error message or null on valid item</returns>
    string Validate(IRegulationItem item);
}