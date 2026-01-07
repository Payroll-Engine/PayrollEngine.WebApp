using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Task = System.Threading.Tasks.Task;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Login() : PageBase(WorkingItems.None), IDisposable
{
    public enum UserLoginState
    {
        Initial,
        InputUser,
        InputTenant,
        SetupPassword,
        InputPassword
    }

    [Inject]
    private PayrollHttpClient HttpClient { get; set; }
    [Inject]
    private IUserService UserService { get; set; }
    [Inject]
    private IUserPasswordService UserPasswordService { get; set; }
    [Inject]
    private IConfiguration Configuration { get; set; }

    #region Title

    /// <summary>
    /// Application title
    /// </summary>
    private string AppTitle { get; set; }

    /// <summary>
    /// Application image
    /// </summary>
    private string AppImage { get; set; }

    /// <summary>
    /// Application dark image
    /// </summary>
    private string AppImageDark { get; set; }

    /// <summary>
    /// Dark mode
    /// </summary>
    private bool IsDarkMode => ThemeService != null && ThemeService.IsDarkMode;

    #endregion

    #region Input Fields

    // user password
    private bool passwordVisible;
    private InputType PasswordType { get; set; } = InputType.Password;
    private string PasswordIcon { get; set; } = Icons.Material.Filled.VisibilityOff;
    // ReSharper disable once UnusedMember.Local
    private void TogglePasswordVisibility()
    {
        if (passwordVisible)
        {
            passwordVisible = false;
            PasswordType = InputType.Password;
            PasswordIcon = Icons.Material.Filled.VisibilityOff;
        }
        else
        {
            passwordVisible = true;
            PasswordType = InputType.Text;
            PasswordIcon = Icons.Material.Filled.Visibility;
        }
    }

    // new user password
    private bool newPasswordVisible;
    private InputType NewPasswordType { get; set; } = InputType.Password;
    private string NewPasswordIcon { get; set; } = Icons.Material.Filled.VisibilityOff;
    // ReSharper disable once UnusedMember.Local
    private void ToggleNewPasswordVisibility()
    {
        if (newPasswordVisible)
        {
            newPasswordVisible = false;
            NewPasswordType = InputType.Password;
            NewPasswordIcon = Icons.Material.Filled.VisibilityOff;
        }
        else
        {
            newPasswordVisible = true;
            NewPasswordType = InputType.Text;
            NewPasswordIcon = Icons.Material.Filled.Visibility;
        }
    }

    // new user password confirmation
    private bool confirmPasswordVisible;
    private InputType PasswordConfirmationType { get; set; } = InputType.Password;
    private string PasswordConfirmationIcon { get; set; } = Icons.Material.Filled.VisibilityOff;
    // ReSharper disable once UnusedMember.Local
    private void TogglePasswordConfirmationVisibility()
    {
        if (confirmPasswordVisible)
        {
            confirmPasswordVisible = false;
            PasswordConfirmationType = InputType.Password;
            PasswordConfirmationIcon = Icons.Material.Filled.VisibilityOff;
        }
        else
        {
            confirmPasswordVisible = true;
            PasswordConfirmationType = InputType.Text;
            PasswordConfirmationIcon = Icons.Material.Filled.Visibility;
        }
    }

    private string UserIdentifier
    {
        get;
        set
        {
            field = value;
            SetSelectedUser();
        }
    }

    private string TenantIdentifier
    {
        get;
        set
        {
            field = value;
            SetSelectedTenant();
        }
    }

    /// <summary>
    /// User password 
    /// </summary>
    private string UserPassword { get; set; }

    /// <summary>
    /// New user password 
    /// </summary>
    private string NewPassword { get; set; }

    /// <summary>
    /// Confirmation of new user password (used for validation purposes)
    /// </summary>
    private string NewPasswordConfirmation { get; set; }

    #endregion

    #region Validation

    private string ErrorMessage { get; set; }

    private UserLoginState LoginState
    {
        get
        {
            if (!ValidUser)
            {
                return UserLoginState.InputUser;
            }
            if (ValidUser && !ValidTenant)
            {
                return UserLoginState.InputTenant;
            }
            if (IsPasswordMissing())
            {
                return UserLoginState.SetupPassword;
            }
            if (ValidUser && ValidTenant)
            {
                return UserLoginState.InputPassword;
            }
            return UserLoginState.Initial;
        }
    }

    private void SetErrorMessage(string message)
    {
        ErrorMessage = message;
    }

    private void ResetErrorMessage() =>
        ErrorMessage = string.Empty;

    #endregion

    #region User

    private const string UserParameter = "user";

    private List<ViewModel.User> Users { get; } = [];

    /// <summary>
    /// User object of selected user identifier string
    /// </summary>
    private ViewModel.User SelectedUser { get; set; }

    /// <summary>
    /// Check if user identifier exists in database
    /// </summary>
    private bool ValidUser =>
        SelectedUser != null;

    /// <summary>
    /// Setup user login by setting user and tenant identifier (form bindings)
    /// </summary>
    /// <param name="user">User object</param>
    /// <param name="tenant">Tenant object</param>
    private void SelectUser(User user, Tenant tenant = null)
    {
        UserIdentifier = user.Identifier;
        if (tenant != null)
        {
            TenantIdentifier = tenant.Identifier;
        }
        SetSelectedUser();
    }

    /// <summary>
    /// Handle change of user identifier
    /// </summary>
    /// <param name="identifier">The new identifier</param>
    // ReSharper disable once UnusedMember.Local
    private void UserSelected(string identifier)
    {
        if (LoginState != UserLoginState.InputUser)
        {
            return;
        }

        // reset any previous errors before checking if user exists
        ResetErrorMessage();

        if (string.IsNullOrEmpty(identifier))
        {
            // nothing to search for
            return;
        }

        var existingUser = GetExistingUser(identifier);
        if (existingUser == null)
        {
            SetErrorMessage(Localizer.Login.UnknownUser(identifier));
            return;
        }

        // setup login page if user is found in database
        // set tenant as well if only one is associated with this user
        Tenant tenant = null;
        var userTenants = GetUserTenants(identifier);
        if (userTenants.Count == 1)
        {
            tenant = userTenants.First();
        }
        SelectUser(existingUser, tenant);
    }

    private void SetSelectedUser()
    {
        var user = Users.FirstOrDefault(u => u.Identifier.Equals(UserIdentifier) &&
                                             u.TenantId == SelectedTenant?.Id);
        if (user != null)
        {
            SelectedUser = user;
        }
        else
        {
            // fallback: look for any user regardless of tenant -> maybe tenant wasn't selected yet
            user = Users.FirstOrDefault(u => u.Identifier.Equals(UserIdentifier));
            if (user != null)
            {
                SelectedUser = user;
            }
        }
    }

    private ViewModel.User GetExistingUser(string identifier) =>
        SearchUsers(identifier).FirstOrDefault();

    private List<ViewModel.User> SearchUsers(string identifier) =>
        Users.Where(x => string.Equals(x.Identifier, identifier)).ToList();

    private void SetupUser()
    {
        if (NavigationManager.TryParseQueryValue(UserParameter, out string user))
        {
            var possibleUsers = Users.Where(c => c.Identifier == user).ToList();
            if (possibleUsers.Any())
            {
                if (NavigationManager.TryParseQueryValue(TenantParameter, out string outTenant))
                {
                    var tenant = GetTenantByName(outTenant);
                    if (tenant != null)
                    {
                        var possibleUser = possibleUsers.FirstOrDefault(c => c.TenantId == tenant.Id);
                        if (possibleUser != null)
                        {
                            SelectUser(possibleUser, tenant);
                        }
                    }
                }
            }
            else
            {
                SetErrorMessage(Localizer.Login.UnknownUser(user));
            }

            // fallback: set identifier to first user if matching with tenant didn't work
            if (string.IsNullOrEmpty(UserIdentifier) && possibleUsers.Any())
            {
                UserIdentifier = user;
            }
        }
    }

    private async Task<bool> SetupUsersAsync()
    {
        foreach (var tenant in Tenants)
        {
            List<ViewModel.User> users;
            try
            {
                users = await UserService.QueryAsync<ViewModel.User>(new(tenant.Id));
            }
            catch (Exception exception)
            {
                Log.Critical($"User service error: {exception}");
                SetErrorMessage(Localizer.Login.UserReadError);
                return false;
            }

            users.ForEach(u => u.TenantId = tenant.Id);
            Users.AddRange(users);
        }
        return true;
    }

    #endregion

    #region Tenant

    private const string TenantParameter = "tenant";

    private List<Tenant> Tenants { get; set; } = [];

    /// <summary>
    /// Tenant object of selected tenant identifier in combobox
    /// </summary>
    private Tenant SelectedTenant { get; set; }

    /// <summary>
    /// Check if tenant identifier exists in database
    /// </summary>
    private bool ValidTenant =>
        SelectedTenant != null;

    private void SetSelectedTenant()
    {
        var tenant = Tenants.FirstOrDefault(t => t.Identifier.Equals(TenantIdentifier));
        if (tenant != null)
        {
            SelectedTenant = tenant;
        }

        SetSelectedUser();
    }

    private void TenantSelected(string identifier)
    {
        if (LoginState != UserLoginState.InputTenant)
        {
            return;
        }

        var tenant = Tenants.FirstOrDefault(t => t.Identifier.Equals(identifier));
        if (tenant == null)
        {
            return;
        }

        SelectedTenant = tenant;
        TenantIdentifier = tenant.Identifier;
    }

    private List<Tenant> GetUserTenants()
    {
        var tenantIds = Users.Where(u => u.Identifier == UserIdentifier).
            Select(x => x.TenantId);
        return Tenants.Where(t => tenantIds.Contains(t.Id)).ToList();
    }

    private List<Tenant> GetUserTenants(string identifier) =>
        Tenants.Where(t => SearchUsers(identifier).
            Select(u => u.TenantId).Contains(t.Id)).ToList();

    private Tenant GetTenantByName(string identifier) =>
        Tenants.FirstOrDefault(t => t.Identifier.Equals(identifier));

    private async Task<bool> SetupTenantsAsync()
    {
        try
        {
            Tenants = (await TenantService.QueryAsync<Tenant>(new()))
                .OrderBy(tenant => tenant.Identifier)
                .ToList();
        }
        catch (Exception exception)
        {
            Log.Critical($"Tenant service error: {exception}");
            SetErrorMessage(Localizer.Login.TenantReadError);
            return false;
        }
        return true;
    }

    #endregion

    #region Password change

    private bool InitialPasswordAvailable()
    {
        if (LoginState != UserLoginState.SetupPassword || string.IsNullOrWhiteSpace(NewPassword))
        {
            return false;
        }
        return PayrollEngine.UserPassword.IsValid(NewPassword);
    }

    /// <summary>
    /// Set initial password
    /// </summary>
    /// <returns>Success message and use login form again</returns>
    // ReSharper disable once UnusedMember.Local
    private async Task SetInitialPasswordAsync()
    {
        if (LoginState != UserLoginState.SetupPassword)
        {
            return;
        }

        ResetErrorMessage();

        // valid format
        if (!PayrollEngine.UserPassword.IsValid(NewPassword))
        {
            SetErrorMessage(Localizer.Login.InvalidPassword);
            return;
        }

        // valid confirmation
        if (!string.Equals(NewPassword, NewPasswordConfirmation))
        {
            SetErrorMessage(Localizer.Login.InvalidPasswordConfirmation);
            return;
        }

        try
        {
            if (!await UserPasswordService.ChangePasswordAsync(SelectedTenant.Id, SelectedUser.Id,
                    new() { NewPassword = NewPassword }))
            {
                SetErrorMessage(Localizer.Login.PasswordChangeError);
                return;
            }
        }
        catch (Exception exception)
        {
            Log.Critical($"Connection to backend failed during password set, error: {exception}");
            SetErrorMessage(Localizer.Login.PasswordChangeError);
            return;
        }

        // set viewmodel property to (a) avoid calling backend again and (b) not misuse password property 
        SelectedUser.PasswordSet = true;
        SelectUser(SelectedUser);
    }

    #endregion

    #region Login & Password

    private bool LoginAvailable() =>
        LoginState == UserLoginState.InputPassword &&
        !string.IsNullOrEmpty(UserPassword);

    /// <summary>
    /// Try to log in user by checking password
    /// </summary>
    /// <returns>Redirect to default/url-parameter page after success</returns>
    // ReSharper disable once UnusedMember.Local
    private async Task LoginAsync()
    {
        if (LoginState != UserLoginState.InputPassword || string.IsNullOrEmpty(UserPassword) ||
            SelectedTenant == null || SelectedTenant == null)
        {
            // don't allow password check if no input given
            return;
        }

        ResetErrorMessage();

        if (!await UserPasswordService.TestPasswordAsync(SelectedTenant.Id, SelectedUser.Id, UserPassword))
        {
            SetErrorMessage(Localizer.Login.InvalidPassword);
            return;
        }

        // login user to session and set tenant if login successful
        await Session.LoginAsync(SelectedTenant, SelectedUser);
        GoToRedirectSite();
    }

    private bool IsPasswordMissing()
    {
        // first check if all inputs are given to determine if user needs new password
        if (SelectedTenant == null || SelectedUser == null ||
            SelectedUser.PasswordSet || !string.IsNullOrEmpty(SelectedUser.Password))
        {
            return false;
        }

        var tenant = GetTenantByName(TenantIdentifier);
        var user = Users.FirstOrDefault(u => u.Identifier == UserIdentifier
                                             && u.TenantId == tenant?.Id);

        // check if user has no password set 
        return user != null && string.IsNullOrEmpty(user.Password) &&
               !user.PasswordSet;
    }

    #endregion

    #region Lifecycle

    private const string RedirectToParameter = "redirectTo";

    private bool BackendAvailable { get; set; }
    private string RedirectPage { get; set; } = PageUrls.Tasks;

    /// <summary>
    /// Set forceLoad to false, otherwise the user session is lost
    /// </summary>
    private void GoToRedirectSite() =>
        NavigationManager.NavigateTo(RedirectPage, forceLoad: false);

    protected override async Task OnPageInitializedAsync()
    {
        // application
        var appConfiguration = Configuration.GetConfiguration<AppConfiguration>();
        AppTitle = appConfiguration.AppTitle ?? SystemSpecification.ApplicationName;
        AppImage = appConfiguration.AppImage;
        AppImageDark = appConfiguration.AppImageDarkMode;

        // backend http configuration
        var configuration = await Configuration.GetHttpConfigurationAsync();
        if (!configuration.Valid())
        {
            ErrorMessage = "Invalid backend connection configuration.";
            Log.Critical(ErrorMessage);
            return;
        }

        // check for connection
        if (!await HttpClient.IsConnectionAvailableAsync(TenantApiEndpoints.TenantsUrl()))
        {
            ErrorMessage = $"Connection not available: {HttpClient.Address}.";
            Log.Critical(ErrorMessage);
            return;
        }

        // connection successful
        BackendAvailable = true;

        // setup tenant cache
        var tenantSetup = await SetupTenantsAsync();
        if (!tenantSetup)
        {
            return;
        }

        // setup tenant users cache
        var userSetup = await SetupUsersAsync();
        if (!userSetup)
        {
            return;
        }

        // setup user (and tenant if given as argument)
        SetupUser();

        // setup redirect page
        if (NavigationManager.TryParseQueryValue(RedirectToParameter, out string redirectTo))
        {
            RedirectPage = redirectTo;
        }

        ThemeService.DarkModeChanged += DarkModeChanged;

        await base.OnPageInitializedAsync();
    }

    private Task DarkModeChanged(object service, bool darkMode) =>
        InvokeAsync(StateHasChanged);

    public void Dispose()
    {
        ThemeService.DarkModeChanged -= DarkModeChanged;
    }
    #endregion

}
