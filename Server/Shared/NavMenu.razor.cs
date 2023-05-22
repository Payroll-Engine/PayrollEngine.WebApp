using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.Presentation;

namespace PayrollEngine.WebApp.Server.Shared;

public partial class NavMenu : IDisposable
{
    [Inject]
    protected UserSession Session { get; set; }

    protected List<PageGroupInfo> PageGroups => PageRegister.PageGroups;
    protected List<PageInfo> Pages => PageRegister.Pages;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // register user change handler
        Session.UserChanged += UserChangedEvent;
    }

    private Task UserChangedEvent(object sender, User user)
    {
        // run the state notification 
        Task.Run(StateHasChanged);
        return Task.CompletedTask;
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