using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using Org.BouncyCastle.Bcpg.Sig;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Shared;

public partial class NavMenu : IDisposable
{
    [Inject]
    private UserSession Session { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }

    protected List<PageGroupInfo> PageGroups { get; private set; }
    protected List<PageInfo> Pages => PageRegister.Pages;

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

    private async Task SetupPageGroupsAsync()
    {
        var pageGroups = new List<PageGroupInfo>();
        foreach (var pageGroup in PageRegister.PageGroups)
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
            return new("No features available<br/>Please contact your administrator");
        }
        return new($"No features available<br/>Please <a href=\"mailto:{adminEmail}\">contact</a> your administrator");
    }

    protected override async Task OnInitializedAsync()
    {
        // register user change handler
        Session.UserChanged += UserChangedEvent;

        await SetupPageGroupsAsync();
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