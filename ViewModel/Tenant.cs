using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Tenant : Client.Model.Tenant, IViewModel,
    IViewAttributeObject, IKeyEquatable<Tenant>
{
    public Tenant()
    {
    }

    public Tenant(Tenant copySource) :
        base(copySource)
    {
    }

    public Tenant(Client.Model.Tenant copySource) :
        base(copySource)
    {
    }

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
    public bool Equals(Tenant compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Tenant);

    public bool EqualKey(Tenant compare) =>
        base.EqualKey(compare);
}