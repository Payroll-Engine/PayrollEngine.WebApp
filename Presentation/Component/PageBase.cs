using System;
using System.Linq;
using System.Text.Json;
using System.Globalization;
using System.Collections.Generic;
using System.Web;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Shared;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public abstract class PageBase(WorkingItems workingItems) : ComponentBase, IDisposable
{
    // access to the main layout (see https://stackoverflow.com/a/66477564)
    [CascadingParameter]
    public MainComponentBase Layout { get; set; }

    [Inject]
    protected NavigationManager NavigationManager { get; set; }
    [Inject]
    protected UserSession Session { get; set; }
    [Inject]
    protected ITenantService TenantService { get; set; }
    [Inject]
    protected IDialogService DialogService { get; set; }
    [Inject]
    protected IUserNotificationService UserNotification { get; set; }
    [Inject]
    protected ILocalizerService LocalizerService { get; set; }
    [Inject]
    protected IJSRuntime JsRuntime { get; set; }

    [Inject]
    private ILogService LogService { get; set; }
    [Inject]
    private IPageService PageService { get; set; }
    [Inject]
    protected ICultureService CultureService { get; set; }
    [Inject]
    protected IThemeService ThemeService { get; set; }

    protected Localizer Localizer => LocalizerService.Localizer;
    protected IValueFormatter ValueFormatter => Session.ValueFormatter;

    // the __builder variable is placed to suppress the ReSharper
    // inspection error 'Cannot resolve symbol '__builder'
    // ReSharper disable once InconsistentNaming
    protected RenderTreeBuilder __builder { get; set; }

    /// <summary>
    /// Page culture
    /// <remarks>[culture by priority]: user > tenant > system</remarks>
    /// </summary>
    protected CultureInfo PageCulture
    {
        get
        {
            // priority 1: user culture
            if (!string.IsNullOrWhiteSpace(User.Culture))
            {
                return new(User.Culture);
            }
            // priority 2: tenant culture
            if (!string.IsNullOrWhiteSpace(Tenant.Culture))
            {
                return new(Tenant.Culture);
            }
            // priority 3: system culture
            return CultureInfo.CurrentCulture;
        }
    }

    #region Working Items

    /// <summary>
    /// The page working items
    /// </summary>
    protected virtual WorkingItems WorkingItems { get; } = workingItems;

    protected bool WorkingItemsFulfilled(int? tenantId, int? payrollId, int? employeeId) =>
        (!WorkingItems.TenantView() && !WorkingItems.TenantChange() || tenantId.HasValue) &&
        (!WorkingItems.PayrollView() && !WorkingItems.PayrollChange() || payrollId.HasValue) &&
        (!WorkingItems.EmployeeView() && !WorkingItems.EmployeeChange() || employeeId.HasValue);

    #endregion

    #region User

    /// <summary>
    ///  Thw working user
    /// </summary>
    protected User User => Session.User;

    /// <summary>
    /// Check for user feature
    /// </summary>
    /// <param name="feature">The feature to test</param>
    /// <returns>True if feature is available</returns>
    protected bool HasFeature(Feature feature) =>
        Session.Tenant != null && Session.UserFeature(feature);

    #endregion

    #region Tenant

    /// <summary>
    /// The working tenant
    /// </summary>
    protected Tenant Tenant => Session.Tenant;

    /// <summary>
    /// The working tenant
    /// </summary>
    protected CultureInfo TenantCulture => GetTenantCulture(Tenant);

    /// <summary>
    /// Get tenant culture
    /// </summary>
    protected static CultureInfo GetTenantCulture(Tenant tenant)
    {
        var culture = CultureInfo.DefaultThreadCurrentCulture ?? CultureInfo.InvariantCulture;
        if (!string.IsNullOrWhiteSpace(tenant?.Culture) &&
            !string.Equals(culture.Name, tenant.Culture))
        {
            culture = new CultureInfo(tenant.Culture);
        }
        return culture;
    }

    /// <summary>
    /// True if tenant is available
    /// </summary>
    protected bool HasTenant => Tenant != null;

    /// <summary>
    /// True if tenant is missing
    /// </summary>
    protected bool IsTenantMissing => !HasTenant;

    /// <summary>
    /// Handler fot tenant change
    /// </summary>
    protected virtual Task OnTenantChangedAsync()
    {
        if (WorkingItems.TenantAvailable())
        {
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Add tenant log entry
    /// </summary>
    /// <param name="message">The log message</param>
    /// <param name="comment">The log comment</param>
    /// <param name="error">The log error</param>
    protected async Task AddTenantLogAsync(string message, string comment = null,
        object error = null)
    {
        var log = new Client.Model.Log
        {
            User = User.Identifier,
            Message = message,
            Comment = comment,
            Error = error != null ? JsonSerializer.Serialize(error) : null
        };
        await LogService.CreateAsync(new(Tenant.Id), log);
    }

    /// <summary>
    /// Ensure unique grid ids within the user storage (web cookies)
    /// do nto use a dot as separator: breaks the grid filtering
    /// </summary>
    /// <param name="gridId">The grid id</param>
    /// <returns>Grid id with tenant prefix</returns>
    protected string GetTenantGridId(string gridId) =>
        HasTenant ? $"{GetTenantGridId()}_{gridId}" : gridId;

    /// <summary>
    /// The base gird id for custom columns
    /// </summary>
    /// <param name="gridId">The grid id</param>
    /// <returns>The base grid id</returns>
    private string GetBaseGridId(string gridId)
    {
        var token = $"{GetTenantGridId()}_";
        if (!gridId.StartsWith(token))
        {
            return gridId;
        }
        return gridId.Replace(token, string.Empty);
    }

    /// <summary>Get the tenant grid id, using the tenant identifier without special characters</summary>
    /// <returns>The tenant grid id</returns>
    private string GetTenantGridId() =>
        Tenant?.Identifier.RemoveSpecialCharacters();

    #endregion

    #region Payroll

    /// <summary>
    /// The working payroll
    /// </summary>
    protected Payroll Payroll => Session.Payroll;

    /// <summary>
    /// True if payroll is available
    /// </summary>
    protected bool HasPayroll => Session.Payroll != null;

    /// <summary>
    /// True if payroll is missing
    /// </summary>
    protected bool IsPayrollMissing =>
        WorkingItems.PayrollChange() && !HasPayroll;

    /// <summary>
    /// Payroll changed handler
    /// </summary>
    /// <param name="payroll">The new payroll</param>
    protected virtual Task OnPayrollChangedAsync(Payroll payroll)
    {
        if (WorkingItems.PayrollAvailable())
        {
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    #endregion

    #region Employee

    /// <summary>
    /// The working employee
    /// </summary>
    protected Employee Employee => Session.Employee;

    /// <summary>
    /// True if employee is available
    /// </summary>
    protected bool HasEmployee => Session.Employee != null;

    /// <summary>
    /// True if employee id missing
    /// </summary>
    protected bool IsEmployeeMissing =>
        WorkingItems.EmployeeChange() && !HasEmployee;

    /// <summary>
    /// Employee changed handler
    /// </summary>
    /// <param name="employee">The new employee</param>
    protected virtual Task OnEmployeeChangedAsync(Employee employee)
    {
        if (WorkingItems.EmployeeAvailable())
        {
            StateHasChanged();
        }
        return Task.CompletedTask;
    }

    #endregion

    #region Grid Configuration

    private Dictionary<string, List<GridColumnConfiguration>> CustomColumns { get; set; }

    /// <summary>
    /// True if custom columns are present
    /// </summary>
    protected bool HasCustomColumns =>
        CustomColumns != null && CustomColumns.Any();

    /// <summary>
    /// Get custom columns configuration
    /// </summary>
    /// <param name="gridId">The grid id</param>
    /// <returns>The grid custom columns</returns>
    protected List<GridColumnConfiguration> GetColumnConfiguration(string gridId)
    {
        if (string.IsNullOrWhiteSpace(gridId))
        {
            throw new ArgumentException(nameof(gridId));
        }

        if (CustomColumns == null)
        {
            return null;
        }

        try
        {
            return CustomColumns.GetValueOrDefault(gridId);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            return null;
        }
    }

    /// <summary>
    /// Setup grid custom columns
    /// </summary>
    /// <param name="gridId"></param>
    protected void SetupCustomColumns(string gridId)
    {
        if (string.IsNullOrWhiteSpace(gridId))
        {
            throw new ArgumentException(nameof(gridId));
        }

        if (Tenant == null || Tenant.Attributes == null)
        {
            return;
        }

        // custom columns stored in tenant
        var attributeName = GetCustomColumnSettingName(GetBaseGridId(gridId));
        if (!Tenant.Attributes.ContainsKey(attributeName))
        {
            return;
        }

        var value = Tenant.Attributes[attributeName];
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.GetValue();
        }
        var customColumnAttribute = value as string;

        // json configuration
        var customColumns = new Dictionary<string, List<GridColumnConfiguration>>();
        if (!string.IsNullOrWhiteSpace(customColumnAttribute))
        {
            try
            {
                var columns = JsonSerializer.Deserialize<List<GridColumnConfiguration>>(customColumnAttribute);
                customColumns.Add(gridId, columns);
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.GetBaseMessage());
            }
        }
        CustomColumns = customColumns;
    }

    private static string GetCustomColumnSettingName(string gridId)
    {
        const string Prefix = "grid.";
        if (gridId.StartsWith(Prefix))
        {
            return gridId;
        }
        return Prefix + gridId;
    }

    #endregion

    #region Navigation & Html

    /// <summary>
    /// Navigate to home page
    /// </summary>
    /// <param name="forceLoad">Enforce page reload</param>
    protected void NavigateHome(bool forceLoad = false) =>
        NavigateTo("/", forceLoad);

    /// <summary>
    /// Navigate to page
    /// </summary>
    /// <param name="uri">The target page address</param>
    /// <param name="forceLoad">Enforce page reload</param>
    protected void NavigateTo(string uri, bool forceLoad = false) =>
        NavigationManager.NavigateTo(uri, forceLoad);

    #endregion

    #region Theme

    /// <summary>The working theme</summary>
    protected MudTheme Theme => ThemeService.Theme;

    /// <summary>The working palette</summary>
#pragma warning disable CS0618
    protected Palette Palette => ThemeService.Palette;

    #endregion

    #region Logging

    protected void LogInformation(string message) =>
        Log.Information(message);

    protected void LogWarning(string message) =>
        Log.Warning(message);

    protected void LogDebug(string message) =>
        Log.Debug(message);

    protected void LogVerbose(string message) =>
        Log.Trace(message);

    protected void LogError(string message) =>
        Log.Error(message);

    protected void LogError(Exception exception) =>
        Log.Error(exception, exception.GetBaseMessage());

    protected void LogError(Exception exception, string message) =>
        Log.Error(exception, message);

    protected void LogFatal(string message) =>
        Log.Critical(message);

    protected void LogFatal(Exception exception, string message) =>
        Log.Critical(exception, message);

    #endregion

    #region Page Title

    private async Task UpdateTitleAsync()
    {
        // page name
        var name = new Uri(NavigationManager.Uri).Segments.Last();
        if (string.IsNullOrWhiteSpace(name) || name == "/")
        {
            return;
        }
        name = name.EnsureStart("/");

        // page
        var pages = PageService.GetPages(Localizer);
        if (pages == null || !pages.Any())
        {
            return;
        }
        var page = pages.FirstOrDefault(
                x => string.Equals(x.PageLink, name.EnsureStart("/"), StringComparison.InvariantCultureIgnoreCase));

        // title
        var title = HttpUtility.UrlDecode(page == null ? name : page.Title);
        var baseLabel = PageService.BaseLabel;
        if (!string.IsNullOrWhiteSpace(baseLabel))
        {
            title = $"{baseLabel} - {title.TrimStart('/').ToPascalSentence(CharacterCase.ToUpper)}";
        }
        await JsRuntime.InvokeVoidAsync("JsFunctions.setDocumentTitle", title);
    }

    #endregion

    #region Lifecycle

    protected bool IsLoading { get; private set; }
    protected bool Initialized { get; private set; }

    private async Task TenantChangedEvent(object sender, Tenant tenant) =>
        await OnTenantChangedAsync();

    private async Task PayrollChangedEvent(object sender, Payroll payroll) =>
        await OnPayrollChangedAsync(payroll);

    private async Task EmployeeChangedEvent(object sender, ViewModel.Employee employee) =>
        await OnEmployeeChangedAsync(employee);

    protected virtual Task OnPageInitializedAsync() => Task.CompletedTask;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;
        try
        {
            // working items
            if (User != null)
            {
                Layout.WorkingItems = WorkingItems;
            }

            // register state change handler
            Session.TenantChanged += TenantChangedEvent;
            Session.PayrollChanged += PayrollChangedEvent;
            Session.EmployeeChanged += EmployeeChangedEvent;

            // derived initialization
            await OnPageInitializedAsync();
            await base.OnInitializedAsync();

            Initialized = true;
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(
                Localizer,
                Localizer.Error.Error,
                exception.GetBaseMessage());
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected virtual Task OnPageAfterRenderAsync(bool firstRender) => Task.CompletedTask;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        try
        {
            if (firstRender)
            {
                await UpdateTitleAsync();
            }

            // derived render
            await OnPageAfterRenderAsync(firstRender);
            await base.OnAfterRenderAsync(firstRender);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(
                Localizer,
                Localizer.Error.Error,
                exception.GetBaseMessage());
        }
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
            Session.EmployeeChanged -= EmployeeChangedEvent;
            Session.PayrollChanged -= PayrollChangedEvent;
            Session.TenantChanged -= TenantChangedEvent;
        }
    }

    #endregion

}