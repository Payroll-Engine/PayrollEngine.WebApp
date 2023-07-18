using System.Globalization;
using System.Linq;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace PayrollEngine.WebApp.Presentation;

public static class LogExtensions
{
    private const LogLevel SystemInfoLogEventLevel = LogLevel.Information;

    public static void UseLog(this IHostApplicationLifetime appLifetime, 
        IApplicationBuilder appBuilder, IHostEnvironment environment)
    {
        // started
        appLifetime.ApplicationStarted.Register(() =>
        {
            Log.Information($"{environment.ApplicationName} started on the URL {GetApplicationAddress(appBuilder)}.");
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