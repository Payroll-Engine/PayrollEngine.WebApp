using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using PayrollEngine.Serilog;
using Serilog;

namespace PayrollEngine.WebApp.Server;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .UseSerilog((hostingContext, loggerConfiguration) =>
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration))
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });


        // logger
        Log.SetLogger(new PayrollLog());

        return builder;
    }
}