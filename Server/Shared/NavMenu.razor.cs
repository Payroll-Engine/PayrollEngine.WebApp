using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Server.Shared;

public partial class NavMenu : IDisposable
{
    [Inject]
    private UserSession Session { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }
    [Inject]
    private Localizer Localizer { get; set; }

    protected List<PageGroupInfo> PageGroups { get; private set; }
    protected List<PageInfo> Pages { get; private set; }

    private Task UserChangedEvent(object sender, User user)
    {
        // run the state notification 
        Task.Run(StateHasChanged);
        return Task.CompletedTask;
    }

    private void GroupExpandChangeAsync(PageGroupInfo pageGroup)
    {
        pageGroup.Expanded = !pageGroup.Expanded;

        // store navigation state
        Task.Run(() => LocalStorage.SetItemAsBooleanAsync($"Navigation{pageGroup.GroupName}", pageGroup.Expanded));
    }

    private async Task SetupPagesAsync()
    {
        // pages
        var register = new PageRegister(Localizer);
        Pages = register.Pages;
        PageGroups = register.PageGroups;

        // page groups
        var pageGroups = new List<PageGroupInfo>();
        foreach (var pageGroup in register.PageGroups)
        {
            var groupSetting = await LocalStorage.GetItemAsBooleanAsync($"Navigation{pageGroup.GroupName}");
            if (groupSetting.HasValue)
            {
                pageGroup.Expanded = groupSetting.Value;
            }
            pageGroups.Add(pageGroup);
        }
        PageGroups = pageGroups;
    }

    private MarkupString GetNoFeatureText()
    {
        var adminEmail = Configuration.GetConfiguration<AppConfiguration>().AdminEmail;
        if (string.IsNullOrEmpty(adminEmail))
        {
            return new($"{Localizer.App.MissingFeatures}<br/>{Localizer.App.AdminContact}");
        }
        return new($"{Localizer.App.MissingFeatures}<br/><a href=\"mailto:{adminEmail}\">{Localizer.App.AdminContact}</a>");
    }

    protected override async Task OnInitializedAsync()
    {
        // register user change handler
        Session.UserChanged += UserChangedEvent;

        await SetupPagesAsync();
        await base.OnInitializedAsync();
    }

    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // un-register session events
            Session.UserChanged -= UserChangedEvent;
        }
    }
}