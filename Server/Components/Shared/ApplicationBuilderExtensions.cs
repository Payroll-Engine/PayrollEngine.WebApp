using System;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;

namespace PayrollEngine.WebApp.Server.Components.Shared;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/>
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Register the application culture for localization middleware
    /// </summary>
    /// <param name="appBuilder">The application builder</param>
    /// <param name="culture">The culture to register</param>
    public static void RegisterCulture(this IApplicationBuilder appBuilder, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        // see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-5.0
        // https://docs.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?view=aspnetcore-5.0
        var supportedCultures = new List<CultureInfo> { culture };
        // Culture will be reset if set directly on CultureInfo.CurrentCulture
        // need to use localization middleware provided by blazor
        appBuilder.UseRequestLocalization(new RequestLocalizationOptions
        {
            DefaultRequestCulture = new(culture, culture),
            // Formatting numbers, dates, etc.
            SupportedCultures = supportedCultures,
            // UI strings that we have localized.
            SupportedUICultures = supportedCultures
        });
    }
}
