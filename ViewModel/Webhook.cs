using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Webhook : Client.Model.Webhook, IViewModel, IViewAttributeObject, IKeyEquatable<Webhook>
{
    public Webhook()
    {
    }

    public Webhook(Webhook copySource) :
        base(copySource)
    {
    }

    public Webhook(Client.Model.Webhook copySource) :
        base(copySource)
    {
    }

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
    public bool Equals(Webhook compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Webhook);

    public bool EqualKey(Webhook compare) =>
        base.EqualKey(compare);
}
