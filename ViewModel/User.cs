using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class User : WebApp.User, IViewModel, IViewAttributeObject, IKeyEquatable<User>
{
    public User()
    {
    }

    public User(User copySource) :
        base(copySource)
    {
    }

    public User(Client.Model.User copySource) :
        base(copySource)
    {
    }

    /// <summary>If password is currently set or not</summary>
    public bool PasswordSet { get; set; }

    /// <summary>The tenant Id</summary>
    public int TenantId { get; set; }

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
    public bool Equals(User compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as User);

    public bool EqualKey(User compare) =>
        base.EqualKey(compare);
}