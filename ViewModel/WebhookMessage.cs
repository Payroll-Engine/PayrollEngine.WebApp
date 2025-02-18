using System;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model webhook message
/// </summary>
public class WebhookMessage : Client.Model.WebhookMessage, IViewModel, IEquatable<WebhookMessage>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public WebhookMessage()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public WebhookMessage(WebhookMessage copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public WebhookMessage(Client.Model.WebhookMessage copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Message request date
    /// </summary>
    public new DateTime? RequestDate
    {
        get => base.RequestDate;
        set => base.RequestDate = value ?? DateTime.MinValue;
    }

    /// <summary>
    /// Message response date
    /// </summary>
    public new DateTime? ResponseDate
    {
        get => base.ResponseDate;
        set => base.ResponseDate = value ?? DateTime.MinValue;
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(WebhookMessage compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as WebhookMessage);
}
