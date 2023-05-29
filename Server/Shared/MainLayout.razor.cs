using System.Diagnostics;
using System.Linq;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using PayrollEngine.WebApp.Presentation;
using ClientModel = PayrollEngine.Client.Model;
using Task = System.Threading.Tasks.Task;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Server.Dialogs;

namespace PayrollEngine.WebApp.Server.Shared;

public abstract class MainLayoutBase : MainComponentBase
{
    [Inject]
    private IConfiguration Configuration { get; set; }
    [Inject]
    private IThemeService ThemeService { get; set; }
    [Inject]
    private ILocalStorageService LocalStorage { get; set; }
    [Inject]
    private IDialogService DialogService { get; set; }

    /// <summary>
    /// Application title
    /// </summary>
    protected string ApplicationTitle { get; private set; }

    protected bool NavigationOpen { get; set; } = true;

    #region Tenant

    /// <summary>
    /// Tenant change state
    /// </summary>
    public bool TenantChangeEnabled =>
        WorkingItems.TenantChange() && Session.Tenants.Count > 1;

    protected async Task WorkingTenantChangedAsync(Tenant tenant)
    {
        if (tenant == null)
        {
            return;
        }
        await Session.ChangeTenantAsync(tenant);
        await InvokeStateHasChangedAsync();
    }

    #endregion

    #region Payroll

    /// <summary>
    /// Payroll change state
    /// </summary>
    public bool PayrollChangeEnabled =>
        Session.Tenant != null && WorkingItems.PayrollChange() && Session.Payrolls.Count > 1;

    protected async Task WorkingPayrollChangedAsync(ClientModel.Payroll payroll)
    {
        await Session.ChangePayrollAsync(payroll);
        await InvokeStateHasChangedAsync();
    }

    protected string GetPayrollName(ClientModel.Payroll payroll) =>
        Session.User.Language.GetLocalization(payroll.NameLocalizations, payroll.Name);

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
        return Session.User.Language.GetLocalization(division.NameLocalizations, division.Name);
    }

    #endregion

    #region Employee

    /// <summary>
    /// Employee change state
    /// </summary>
    public bool EmployeeChangeEnabled =>
        Session.Tenant != null && WorkingItems.EmployeeChange() && Session.Employees.Count > 1;

    protected async Task WorkingEmployeeChangedAsync(Employee workingEmployee)
    {
        if (workingEmployee != null)
        {
            await Session.ChangeEmployeeAsync(workingEmployee);
            await InvokeStateHasChangedAsync();
        }
    }

    #endregion

    #region User

    protected async Task SetupAccountAsync()
    {
        if (Session.User == null)
        {
            return;
        }

        // report parameters
        var parameters = new DialogParameters
        {
            { nameof(UserSettingsDialog.Tenant), Session.Tenant },
            { nameof(UserSettingsDialog.User), Session.User }
        };
        await DialogService.ShowAsync<UserSettingsDialog>("User Account", parameters);
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

    protected bool StartupDarkMode()
    {
        var startupConfiguration = Configuration.GetConfiguration<StartupConfiguration>();
        return startupConfiguration != null && startupConfiguration.DarkMode;
    }

    private async Task SetupThemeAsync()
    {
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
    }

    #endregion

    #region Navigation

    protected async Task NavigationToggleAsync()
    {
        NavigationOpen = !NavigationOpen;

        // store navigation state
        await LocalStorage.SetItemAsBooleanAsync("NavigationOpen", NavigationOpen);
    }

    private async Task SetupNavigationAsync()
    {
        // navigation state
        var navigationOpen = await LocalStorage.GetItemAsBooleanAsync("NavigationOpen");
        if (navigationOpen.HasValue)
        {
            NavigationOpen = navigationOpen.Value;
        }
    }

    protected void NavigateToTasks() =>
        NavigateTo(PageUrls.Tasks);

    #endregion

    #region Lifecycle

    private const string CreatedBy = "Software Consulting Giannoudis";
    protected async Task AboutAsync()
    {
        FileVersionInfo assemblyInfo = FileVersionInfo.GetVersionInfo(GetType().Assembly.Location);
        FileVersionInfo backendAssemblyInfo = FileVersionInfo.GetVersionInfo(typeof(PayrollEngine.SystemSpecification).Assembly.Location);
        await UserNotificationService.ShowMessageBoxAsync(ApplicationTitle,
            new MarkupString($"Web App Version: {assemblyInfo.ProductVersion}<br />" +
                             $"Backend Version: {backendAssemblyInfo.ProductVersion}<br /><br />" +
                             $"Created by {CreatedBy}<br />"), yesText: "OK");
    }

    protected MudTheme AppTheme { get; set; }
    protected override async Task OnInitializedAsync()
    {
        // application
        var appConfiguration = Configuration.GetConfiguration<AppConfiguration>();
        ApplicationTitle = appConfiguration.AppTitle ?? SystemSpecification.ApplicationName;

        // theme
        AppTheme = ThemeService.Theme;
        IsDarkMode = StartupDarkMode();
        await SetupThemeAsync();
        // global service
        ThemeService.IsDarkMode = IsDarkMode;

        // navigation
        await SetupNavigationAsync();
        await base.OnInitializedAsync();
    }

    protected UserNotification Notification { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !UserNotificationService.IsInitialized)
        {
            // initialize user notifications
            UserNotificationService.Initialize(Notification);
            Session.UserNotification = Notification;
        }
        await base.OnAfterRenderAsync(firstRender);

    }

    #endregion

}