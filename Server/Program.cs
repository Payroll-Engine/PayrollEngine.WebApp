using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using Serilog;
using SysLog = Serilog;
using PayrollEngine.Serilog;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Server.Components;
using PayrollEngine.WebApp.Server.Components.Shared;

namespace PayrollEngine.WebApp.Server;

public class Program
{
    public static async Task Main(string[] args)
    {
        // configuration
        var configuration = new ConfigurationBuilder()
            .AddCommandLine(args)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .AddUserSecrets(typeof(Program).Assembly)
            .Build();

        // system logger
        SysLog.Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            var appConfig = builder.Configuration.GetConfiguration<AppConfiguration>();
            var startupConfig = builder.Configuration.GetConfiguration<StartupConfiguration>();

             // system logs
            builder.Configuration.SetupSerilog();
            Log.Information("Payroll Engine Web Application started.");

            // app logs
            if (appConfig.LogHttpRequests)
            {
                builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
                    .ReadFrom.Configuration(builder.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext());
            }

            // app services
            await AddAppServicesAsync(builder);

            await using var app = builder.Build();

            // app logs
            app.UseHostLog();

           // culture
            if (startupConfig != null)
            {
                var culture = app.Services.GetService<ICultureService>()
                    .GetCulture(startupConfig.StartupCulture);
                app.RegisterCulture(culture.CultureInfo);
            }

            // configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                // app.UseHsts();
            }

            // http request logs
            if (appConfig.LogHttpRequests)
            {
                app.UseSerilogRequestLogging();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            await app.RunAsync();
        }
        catch (Exception exception)
        {
            Log.Critical(exception, exception.GetBaseException().Message);
        }
        finally
        {
            Log.Information("Payroll Engine Web Application terminated.");
            await SysLog.Log.CloseAndFlushAsync();
        }
    }

    private static async Task AddAppServicesAsync(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        // app services
        await services.AddAppServicesAsync(configuration);

        // server
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();

        var appConfiguration = configuration.GetConfiguration<AppConfiguration>();
        services.AddSession(options =>
        {
            // Set a short timeout for easy testing
            options.IdleTimeout = appConfiguration.SessionTimeout;
            options.Cookie.HttpOnly = true;
            // Make the session cookie essential
            options.Cookie.IsEssential = true;
        });

        // Ongoing issue, default behaviour is currently throwing an error if json serializer is trying to convert blank string
        // See open issue here: https://github.com/dotnet/aspnetcore/issues/8847#issuecomment-477319495
        services.AddMvc(options =>
        {
            var noContentFormatter = options.OutputFormatters.OfType<HttpNoContentOutputFormatter>().FirstOrDefault();
            if (noContentFormatter != null)
            {
                noContentFormatter.TreatNullValueAsNoContent = false;
            }
        });

        services.AddRazorComponents(options =>
                options.DetailedErrors = builder.Environment.IsDevelopment())
            .AddInteractiveServerComponents();
        services.AddBlazoredLocalStorage();

        // UI
        services.AddMudServices(options =>
        {
            // user notification messages
            options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            options.PopoverOptions.ThrowOnDuplicateProvider = false;
        });
    }
}