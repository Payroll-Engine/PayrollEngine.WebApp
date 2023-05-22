using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Builder;

namespace PayrollEngine.WebApp.Server.Shared;

public static class ApplicationBuilderExtensions
{
    public static void RegisterCulture(this IApplicationBuilder appBuilder, string cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return;
        }

        // see: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/localization?view=aspnetcore-5.0
        // https://docs.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?view=aspnetcore-5.0
        var culture = CultureTool.GetCulture(cultureName);
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