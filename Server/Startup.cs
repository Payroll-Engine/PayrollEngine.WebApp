using System;
using System.Linq;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Services;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Server.Shared;
using Serilog.Extensions.Logging;

// Fresh blazor apps have different startup file structure (Startup.cs+Program.cs combined)
// currently "old" structure is 100% supported, follow upgrade guide if this should change in future
// upgrade guide: https://andrewlock.net/exploring-dotnet-6-part-12-upgrading-a-dotnet-5-startup-based-app-to-dotnet-6/

namespace PayrollEngine.WebApp.Server;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    private IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        var appConfiguration = Configuration.GetConfiguration<AppConfiguration>();

        // app services
        services.AddAppServices(Configuration);

        // server
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();

        var timeout = appConfiguration?.SessionTimeout > 0 ? appConfiguration.SessionTimeout : AppConfiguration.DefaultSessionTimeout;
        services.AddSession(options =>
        {
            // Set a short timeout for easy testing
            options.IdleTimeout = TimeSpan.FromMinutes(timeout);
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

        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddBlazoredLocalStorage();

        // UI
        services.AddMudServices(options =>
        {
            // user notification messages
            options.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
            options.PopoverOptions.ThrowOnDuplicateProvider = false;
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder appBuilder, IWebHostEnvironment environment,
        ILoggerFactory loggerFactory, IHostApplicationLifetime appLifetime)
    {
        // logging
        loggerFactory.AddProvider(new SerilogLoggerProvider());
        appLifetime.UseLog(appBuilder, environment);

        if (environment.IsDevelopment())
        {
            appBuilder.UseDeveloperExceptionPage();
        }
        else
        {
            appBuilder.UseExceptionHandler("/Error");
            // The default value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            appBuilder.UseHsts();
        }

        appBuilder.UseSession();
        appBuilder.UseHttpsRedirection();
        appBuilder.UseStaticFiles();

        // set culture
        var appConfiguration = Configuration.GetConfiguration<AppConfiguration>();
        if (appConfiguration != null)
        {
            appBuilder.RegisterCulture(appConfiguration.Culture);
        }

        appBuilder.UseRouting();

        appBuilder.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
        });
    }
}