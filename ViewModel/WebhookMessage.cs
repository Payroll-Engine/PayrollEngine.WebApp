﻿using System;

namespace PayrollEngine.WebApp.ViewModel;

public class WebhookMessage : Client.Model.WebhookMessage, IViewModel, IEquatable<WebhookMessage>
{
    public WebhookMessage()
    {
    }

    public WebhookMessage(WebhookMessage copySource) :
        base(copySource)
    {
    }

    public WebhookMessage(Client.Model.WebhookMessage copySource) :
        base(copySource)
    {
    }

    public new DateTime? RequestDate
    {
        get => base.RequestDate;
        set => base.RequestDate = value ?? DateTime.MinValue;
    }

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
