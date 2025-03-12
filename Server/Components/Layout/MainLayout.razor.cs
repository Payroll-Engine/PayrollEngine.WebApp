using System.Linq;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Dialogs;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using ClientModel = PayrollEngine.Client.Model;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Components.Layout;

public abstract class MainLayoutBase : MainComponentBase
{
    [Inject]
    protected ILocalizerService LocalizerService { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }
    [Inject]
    private IThemeService ThemeService { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }
    [Inject]
    private PayrollHttpClient PayrollHttpClient { get; set; }
#if DEBUG
    [Inject]
    private UserSessionBootstrap UserSessionBootstrap { get; set; }
#endif

    protected Localizer Localizer => LocalizerService.Localizer;

    /// <summary>
    /// Application title
    /// </summary>
    protected string AppTitle { get; private set; }

    /// <summary>
    /// Application image
    /// </summary>
    private string AppImage { get; set; }

    /// <summary>
    /// Application image dark mode
    /// </summary>
    private string AppImageDarkMode { get; set; }

    /// <summary>
    /// Current application image
    /// </summary>
    protected string CurrentAppImage =>
        IsDarkMode ? AppImageDarkMode : AppImage;

    protected bool NavigationOpen { get; set; }

    #region Tenant

    protected bool TenantAvailable => WorkingItems.TenantAvailable();

    /// <summary>
    /// Tenant change state
    /// </summary>
    protected bool TenantChangeEnabled =>
        WorkingItems.TenantChange() && Session.Tenants.Count > 1;

    protected async Task WorkingTenantChangedAsync(Tenant tenant)
    {
        // update authorization
        UpdateAuthorization(tenant?.Identifier);
        if (tenant == null)
        {
            return;
        }

        // update session
        await Session.ChangeTenantAsync(tenant);
        // changed state
        StateHasChanged();
    }

    #endregion

    #region Payroll

    protected bool PayrollAvailable => WorkingItems.PayrollAvailable();

    /// <summary>
    /// Payroll change state
    /// </summary>
    protected bool PayrollChangeEnabled =>
        Session.Tenant != null && WorkingItems.PayrollChange() && Session.Payrolls.Count > 1;

    protected async Task WorkingPayrollChangedAsync(ClientModel.Payroll payroll)
    {
        await Session.ChangePayrollAsync(payroll);
        // changed state
        StateHasChanged();
    }

    protected string GetPayrollName(ClientModel.Payroll payroll) =>
        Session.User.Culture.GetLocalization(payroll.NameLocalizations, payroll.Name);

    protected string GetPayrollDivisionName(ClientModel.Payroll payroll)
    {
        if (string.IsNullOrWhiteSpace(payroll.DivisionName))
        {
            Log.Warning($"Payroll {payroll.Name} without division");
            return null;
        }
        var division = Session.Divisions.FirstOrDefault(x => string.Equals(x.Name, payroll.DivisionName));
        if (division == null)
        {
            Log.Warning($"Unknown division {payroll.DivisionName} in payroll {payroll.Name} ({payroll.Id})");
            return null;
        }
        return Session.User.Culture.GetLocalization(division.NameLocalizations, division.Name);
    }

    #endregion

    #region Employee

    protected bool EmployeeAvailable => WorkingItems.EmployeeAvailable();

    /// <summary>
    /// Employee change state
    /// </summary>
    protected bool EmployeeChangeEnabled =>
        Session.Tenant != null && WorkingItems.EmployeeChange() && Session.Employees.Count > 1;

    protected async Task WorkingEmployeeChangedAsync(Employee workingEmployee)
    {
        if (workingEmployee != null)
        {
            await Session.ChangeEmployeeAsync(workingEmployee);
            // changed state
            StateHasChanged();
        }
    }

    #endregion

    #region User

    protected async Task ChangeUserSettingsAsync()
    {
        if (Session.User == null)
        {
            return;
        }

        var culture = Session.User.Culture;

        // dialog parameters
        var parameters = new DialogParameters
        {
            { nameof(UserSettingsDialog.Tenant), Session.Tenant },
            { nameof(UserSettingsDialog.User), Session.User }
        };
        var result = await (await DialogService.ShowAsync<UserSettingsDialog>(
            Localizer.User.UserSettings, parameters)).Result;
        if (result == null || result.Canceled)
        {
            return;
        }

        // update user culture
        Session.UpdateUserState();

        // refresh the page
        if (!string.Equals(culture, Session.User.Culture))
        {
            NavigationManager.NavigateTo(NavigationManager.Uri, true);
        }
    }

    #endregion

    #region Login/Logout

    protected bool UserLoginEnabled()
    {
#if DEBUG
        var startupConfiguration = Configuration.GetConfiguration<StartupConfiguration>();
        return startupConfiguration == null || !startupConfiguration.AutoLogin;
#else
        return true;
#endif
    }

    protected async Task LogoutAsync()
    {
        await Session.LogoutAsync();
        NavigationManager.NavigateTo("/", true);
    }

    #endregion

    #region Theme

    protected MudThemeProvider ThemeProvider { get; set; }
    protected bool IsDarkMode { get; set; }

    protected async Task ToggleThemeAsync()
    {
        // switch dark mode
        IsDarkMode = !IsDarkMode;
        // global service
        ThemeService.IsDarkMode = IsDarkMode;

        // store theme dark setting
        await LocalStorage.SetItemAsBooleanAsync("DarkTheme", IsDarkMode);
    }

    private void SetupTheme(bool? darkMode)
    {
        AppTheme = ThemeService.Theme;
        // system dark mode at bootstrap
        IsDarkMode = darkMode ?? Platform.GetDarkMode();
        ThemeService.IsDarkMode = IsDarkMode;
    }

    private async Task InitThemeAsync()
    {
        // user dark mode
        bool? darkModeSetting = null;
        if (await LocalStorage.ContainKeyAsync("DarkTheme"))
        {
            var lastMode = await LocalStorage.GetItemAsStringAsync("DarkTheme");
            if (bool.TryParse(lastMode, out var lastDarkMode))
            {
                darkModeSetting = lastDarkMode;
            }
        }
        else
        {
            darkModeSetting = await ThemeProvider.GetSystemPreference();
        }
        var darkMode = darkModeSetting ?? await ThemeProvider.GetSystemPreference();
        if (darkMode != IsDarkMode)
        {
            IsDarkMode = darkMode;
            StateHasChanged();
        }
        ThemeService.IsDarkMode = IsDarkMode;
    }

    #endregion

    #region Navigation

    protected async Task NavigationToggleAsync()
    {
        NavigationOpen = !NavigationOpen;

        // store navigation state
        await LocalStorage.SetItemAsBooleanAsync("NavigationOpen", NavigationOpen);
    }

    private async Task InitNavigationAsync()
    {
        // restore navigation state
        var navigationOpen = await LocalStorage.GetItemAsBooleanAsync("NavigationOpen");
        if (navigationOpen.HasValue)
        {
            NavigationOpen = navigationOpen.Value;
            StateHasChanged();
        }
    }

    protected void NavigateToTasks() =>
        NavigateTo(PageUrls.Tasks);

    #endregion

    #region Lifecycle

    protected async Task AboutAsync()
    {
        // dialog setup
        var parameters = new DialogParameters
        {
            { nameof(AboutDialog.AppTitle), AppTitle },
            { nameof(AboutDialog.AppImage), CurrentAppImage },
            { nameof(AboutDialog.ProductUrl), Configuration.GetConfiguration<AppConfiguration>().ProductUrl },
            { nameof(AboutDialog.AdminEmail), Configuration.GetConfiguration<AppConfiguration>().AdminEmail }
        };
        var options = new DialogOptions
        {
            NoHeader = true
        };
        await DialogService.ShowAsync<AboutDialog>(null, parameters, options);
    }

    private void UpdateAuthorization(string tenantIdentifier)
    {
        if (string.IsNullOrWhiteSpace(tenantIdentifier))
        {
            // remove
            PayrollHttpClient.RemoveTenantAuthorization();
        }
        else
        {
            // set
            PayrollHttpClient.SetTenantAuthorization(tenantIdentifier);
        }
    }

    protected MudTheme AppTheme { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        // application
        var configuration = Configuration.GetConfiguration<AppConfiguration>();
        AppTitle = configuration.AppTitle ?? SystemSpecification.ApplicationName;
        AppImage = configuration.AppImage;
        AppImageDarkMode = configuration.AppImageDarkMode;

        // user session bootstrap
#if DEBUG
        UserSessionBootstrap.Update(Session);
#endif

        // authorization
        UpdateAuthorization(Session.Tenant?.Identifier);

        // theme
        SetupTheme(configuration.DarkMode);

        await base.OnInitializedAsync();
    }

    protected UserNotification Notification { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // theme
            await InitThemeAsync();
            await InitNavigationAsync();

            // notification
            if (!UserNotificationService.IsInitialized)
            {
                UserNotificationService.Initialize(Notification);
                Session.UserNotification = Notification;
            }
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion

}