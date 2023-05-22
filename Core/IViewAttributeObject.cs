
namespace PayrollEngine.WebApp;

/// <summary>
/// Object with dynamic object attributes
/// </summary>
public interface IViewAttributeObject : IAttributeObject
{
    /// <summary>
    /// Get string attribute value
    /// </summary>
    /// <param name="name">The attribute key</param>
    /// <returns>The attribute string value</returns>
    string GetStringAttribute(string name);

    /// <summary>
    /// Get numeric attribute value
    /// </summary>
    /// <param name="name">The attribute key</param>
    /// <returns>The attribute numeric value</returns>
    decimal GetNumericAttribute(string name);

    /// <summary>
    /// Get boolean attribute value
    /// </summary>
    /// <param name="name">The attribute key</param>
    /// <returns>The attribute string value</returns>
    bool GetBooleanAttribute(string name);
}