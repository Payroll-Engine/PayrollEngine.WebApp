using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Shared;

public partial class NavMenu : IDisposable
{
    [Inject]
    private UserSession Session { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }

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