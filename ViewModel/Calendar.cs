using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model calendar
/// </summary>
public class Calendar : Client.Model.Calendar, IViewModel,
    IViewAttributeObject, IKeyEquatable<Calendar>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Calendar()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Calendar(Calendar copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Calendar(Client.Model.Calendar copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Get localized calendar name
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedName(string culture) =>
        culture.GetLocalization(NameLocalizations, Name);

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? 0;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? false;

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Calendar compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Calendar);

    public bool EqualKey(Calendar compare) =>
        base.EqualKey(compare);
}
