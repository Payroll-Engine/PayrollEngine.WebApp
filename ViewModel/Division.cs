using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Division : Client.Model.Division, IViewModel,
    IViewAttributeObject, IKeyEquatable<Division>
{
    public Division()
    {
    }

    public Division(Division copySource) :
        base(copySource)
    {
    }

    public Division(Client.Model.Division copySource) :
        base(copySource)
    {
    }

    public string GetLocalizedName(string culture) =>
        culture.GetLocalization(NameLocalizations, Name);

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
    public bool Equals(Division compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Division);

    public bool EqualKey(Division compare) =>
        base.EqualKey(compare);
}
