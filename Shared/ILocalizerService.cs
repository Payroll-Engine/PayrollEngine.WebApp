﻿namespace PayrollEngine.WebApp.Shared;

/// <summary>
/// Localizer service
/// </summary>
public interface ILocalizerService
{
    /// <summary>
    /// Localizer
    /// </summary>
    Localizer Localizer { get; }

    /// <summary>
    /// Invalidate the localizer
    /// </summary>
    void Invalidate();
}