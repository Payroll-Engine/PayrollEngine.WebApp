using System;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public abstract class MainComponentBase : LayoutComponentBase
{

    [Inject]
    protected UserSession Session { get; set; }
    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected IUserNotificationService UserNotificationService { get; set; }
    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    protected bool HasFeature(Feature feature) =>
        Session.UserFeature(feature);


    #region Working Items

    private WorkingItems workingItems;
    public WorkingItems WorkingItems
    {
        get => workingItems;
        set
        {
            if (value == workingItems)
            {
                return;
            }
            workingItems = value;
            StateHasChanged();
        }
    }

    #endregion

    #region Navigation & Html

    public void NavigateHome(bool forceLoad = false) =>
        NavigateTo("/", forceLoad);

    protected void NavigateTo(string uri, bool forceLoad = false) =>
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

}