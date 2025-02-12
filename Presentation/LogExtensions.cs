using System.Linq;
using System.Globalization;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace PayrollEngine.WebApp.Presentation;

public static class LogExtensions
{
    private const LogLevel SystemInfoLogEventLevel = LogLevel.Information;

    public static void UseHostLog(this IApplicationBuilder builder)
    {
        var appLifetime = builder.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        if (appLifetime == null)
        {
            return;
        }
        var environment = builder.ApplicationServices.GetRequiredService<IHostEnvironment>();
        if (environment == null)
        {
            return;
        }

        // started
        appLifetime.ApplicationStarted.Register(() =>
        {
            Log.Information($"{environment.ApplicationName} started on the URL {GetApplicationAddress(builder)}.");
            if (Log.IsEnabled(SystemInfoLogEventLevel))
            {
                Log.Write(SystemInfoLogEventLevel, $"Current culture: {CultureInfo.CurrentCulture}");
                Log.Write(SystemInfoLogEventLevel, $"Current UI culture: {CultureInfo.CurrentUICulture}");
            }
        });
        // stopping
        appLifetime.ApplicationStopping.Register(() =>
        {
            Log.Information($"{environment.ApplicationName} is stopping...");
        });
        // stopped
        appLifetime.ApplicationStopped.Register(() =>
        {
            Log.Information($"{environment.ApplicationName} stopped.");
        });
    }

    private static string GetApplicationAddress(IApplicationBuilder appBuilder)
    {
        var serverAddressesFeature = appBuilder.ServerFeatures.Get<IServerAddressesFeature>();
        var address = serverAddressesFeature.Addresses.First().RemoveFromEnd("/");
        return address;
    }
}