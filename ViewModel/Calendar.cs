using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Calendar : Client.Model.Calendar, IViewModel,
    IViewAttributeObject, IKeyEquatable<Calendar>
{
    public Calendar()
    {
    }

    public Calendar(Calendar copySource) :
        base(copySource)
    {
    }

    public Calendar(Client.Model.Calendar copySource) :
        base(copySource)
    {
    }

    public string GetLocalizedName(Language language) =>
        language.GetLocalization(NameLocalizations, Name);

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? default;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? default;

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
