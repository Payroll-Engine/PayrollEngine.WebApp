using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation;

public abstract class MainComponentBase : LayoutComponentBase, IDisposable
{
    [Inject]
    protected UserSession Session { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected IUserNotificationService UserNotificationService { get; set; }
    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    protected WorkingItems WorkingItems => Session.WorkingItems;

    protected bool IsInitialized { get; private set; }

    public bool HasFeature(Feature feature) =>
        Session.UserFeature(feature);


    #region Working Items

    protected virtual async Task WorkingItemsChanged(WorkingItems workingItems)
    {
        await InvokeStateHasChangedAsync();
    }

    #endregion

    #region Navigation & Html

    public void NavigateHome(bool forceLoad = false) =>
        NavigateTo("/", forceLoad);

    public void NavigateTo(string uri, bool forceLoad = false) =>
        NavigationManager.NavigateTo(uri, forceLoad);

    #endregion

    #region User Notifications

    public async Task ShowInformationAsync(string message) =>
        await UserNotificationService.ShowInformationAsync(message);

    public async Task ShowSuccessAsync(string message) =>
        await UserNotificationService.ShowSuccessAsync(message);

    public async Task ShowWarningAsync(string message) =>
        await UserNotificationService.ShowWarningAsync(message);

    public async Task ShowErrorAsync(string message) =>
        await UserNotificationService.ShowErrorAsync(message);

    public async Task ShowErrorAsync(Exception exception, string message = null) =>
        await UserNotificationService.ShowErrorAsync(exception, message);

    public async Task ShowAlertAsync(string message) =>
        await JsRuntime.Alert(message);

    #endregion

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // register state change handler
        Session.WorkingItemsChanged += async (_, e) =>
        {
            await WorkingItemsChanged(e);
        };

        // initialization state
        IsInitialized = true;
    }

    // see https://stackoverflow.com/questions/56477829/how-to-fix-the-current-thread-is-not-associated-with-the-renderers-synchroniza
    // answer: https://stackoverflow.com/a/60353701
    protected async Task InvokeStateHasChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
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
            // un-register state change handler
            Session.WorkingItemsChanged -= async (_, e) =>
            {
                await WorkingItemsChanged(e);
            };
        }
    }
}