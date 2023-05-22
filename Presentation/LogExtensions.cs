using System.Globalization;
using Microsoft.Extensions.Hosting;

namespace PayrollEngine.WebApp.Presentation;

public static class LogExtensions
{
    private const LogLevel SystemInfoLogEventLevel = LogLevel.Information;

    public static void UseLog(this IHostApplicationLifetime appLifetime, IHostEnvironment environment)
    {
        // started
        appLifetime.ApplicationStarted.Register(() =>
        {
            Log.Information($"{environment.ApplicationName} started.");
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
}