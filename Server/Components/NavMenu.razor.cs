using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Server.Components;

public partial class NavMenu : IDisposable
{
    [Inject]
    private UserSession Session { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }
    [Inject]
    protected ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private List<PageGroupInfo> PageGroups { get; set; }
    private List<PageInfo> Pages { get; set; }

    private string GetAdminEmail() =>
        Configuration.GetConfiguration<AppConfiguration>().AdminEmail;

    private Task UserChangedEvent(object sender, User user)
    {
        // run the state notification 
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async Task UserStateChangedEvent(object sender, User user)
    {
        await SetupPagesAsync();
        await InvokeAsync(StateHasChanged);
    }

    private async Task GroupExpandChange(PageGroupInfo pageGroup)
    {
        pageGroup.Expanded = !pageGroup.Expanded;

        // store navigation state
        await LocalStorage.SetItemAsBooleanAsync($"Navigation{pageGroup.GroupName}", pageGroup.Expanded);
    }

    private async Task SetupPagesAsync()
    {
        var register = new PageRegister(Localizer);
        Pages = register.Pages;
        PageGroups = register.PageGroups;

        foreach (var pageGroup in PageGroups)
        {
            var groupSetting = await LocalStorage.GetItemAsBooleanAsync($"Navigation{pageGroup.GroupName}");
            if (groupSetting.HasValue && groupSetting.Value != pageGroup.Expanded)
            {
                pageGroup.Expanded = groupSetting.Value;
            }
        }
    }

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        // register user change handler
        Session.UserChanged += UserChangedEvent;
        Session.UserStateChanged += UserStateChangedEvent;
        await SetupPagesAsync();
        await base.OnInitializedAsync();
    }

    void IDisposable.Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            // un-register session events
            Session.UserChanged -= UserChangedEvent;
            Session.UserStateChanged -= UserStateChangedEvent;
        }
    }

    #endregion

}