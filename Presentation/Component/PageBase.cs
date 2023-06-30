using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using MudBlazor;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Shared;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Component;

public abstract class PageBase : ComponentBase, IDisposable
{
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
    protected Localizer Localizer { get; set; }
    [Inject]
    private ILogService LogService { get; set; }
    [Inject]
    private IThemeService ThemeService { get; set; }

    protected IValueFormatter ValueFormatter => Session.ValueFormatter;

    protected PageBase(WorkingItems workingItems)
    {
        WorkingItems = workingItems;
    }

    // the __builder variable is placed to suppress the ReSharper
    // inspection error 'Cannot resolve symbol '__builder'
    // ReSharper disable once InconsistentNaming
    protected RenderTreeBuilder __builder { get; set; }

    /// <summary>
    /// The working culture by priority: user > tenant > system (UI)
    /// </summary>
    protected string Culture =>
        User.Culture ??
        Tenant.Culture ??
        CultureInfo.CurrentUICulture.Name;

    #region Working Items

    /// <summary>
    /// The page working items
    /// </summary>
    protected virtual WorkingItems WorkingItems { get; }

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
    protected virtual async Task OnTenantChangedAsync()
    {
        if (WorkingItems.TenantAvailable())
        {
            await StateHasChangedAsync();
        }
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
    protected virtual async Task OnPayrollChangedAsync(Payroll payroll)
    {
        if (WorkingItems.PayrollAvailable())
        {
            await StateHasChangedAsync();
        }
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
    protected virtual async Task OnEmployeeChangedAsync(Employee employee)
    {
        if (WorkingItems.EmployeeAvailable())
        {
            await StateHasChangedAsync();
        }
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
            return CustomColumns.TryGetValue(gridId, out var customColumn) ? customColumn : null;
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

    #region Lifecycle

    private async Task TenantChangedEvent(object sender, Tenant tenant) =>
        await OnTenantChangedAsync();

    private async Task PayrollChangedEvent(object sender, Payroll payroll) =>
        await OnPayrollChangedAsync(payroll);

    private async Task EmployeeChangedEvent(object sender, ViewModel.Employee employee) =>
        await OnEmployeeChangedAsync(employee);

    // see https://stackoverflow.com/questions/56477829/how-to-fix-the-current-thread-is-not-associated-with-the-renderers-synchroniza
    // answer: https://stackoverflow.com/a/60353701
    private async Task StateHasChangedAsync()
    {
        await InvokeAsync(StateHasChanged);
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // register state change handler
        Session.TenantChanged += TenantChangedEvent;
        Session.PayrollChanged += PayrollChangedEvent;
        Session.EmployeeChanged += EmployeeChangedEvent;

        // working items
        await Session.SetWorkingItemsAsync(WorkingItems);
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