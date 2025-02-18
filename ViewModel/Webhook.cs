using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model webhook
/// </summary>
public class Webhook : Client.Model.Webhook, IViewModel, IViewAttributeObject, IKeyEquatable<Webhook>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public Webhook()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public Webhook(Webhook copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
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
        Attributes?.GetDecimalAttributeValue(name) ?? 0;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? false;

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

    /// <inheritdoc />
    public bool EqualKey(Webhook compare) =>
        base.EqualKey(compare);
}
