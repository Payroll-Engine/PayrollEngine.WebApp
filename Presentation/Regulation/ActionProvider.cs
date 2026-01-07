using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;

namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// Provides regulation actions
/// </summary>
public class ActionProvider
{
    /// <summary>
    /// The tenant service
    /// </summary>
    private ITenantService TenantService { get; }

    /// <summary>
    /// The payroll service
    /// </summary>
    private IPayrollService PayrollService { get; }

    /// <summary>
    /// The tenant id
    /// </summary>
    private int TenantId { get; }

    /// <summary>
    /// The payroll id
    /// </summary>
    private int PayrollId { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="tenantService">The tenant service</param>
    /// <param name="payrollService">The payroll service</param>
    /// <param name="tenantId">The tenant id</param>
    /// <param name="payrollId">The payroll id</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ActionProvider(ITenantService tenantService, IPayrollService payrollService,
        int tenantId, int payrollId)
    {
        TenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        PayrollService = payrollService ?? throw new ArgumentNullException(nameof(payrollService));
        TenantId = tenantId;
        PayrollId = payrollId;
    }

    // action cache
    private List<ActionInfo> actions;

    /// <summary>
    /// Get action of function type
    /// </summary>
    /// <param name="functionType">The function type</param>
    /// <param name="category">The action category</param>
    /// <returns>Function actions</returns>
    public async Task<List<ActionInfo>> GetActions(FunctionType functionType, string category = null)
    {
        // action setup
        if (actions == null || !actions.Any())
        {
            actions = await LoadActions(functionType);
        }

        // category filter
        if (!string.IsNullOrWhiteSpace(category))
        {
            return actions.Where(x => x.FunctionType == functionType &&
                                      x.Categories != null && x.Categories.Contains(category)).ToList();
        }
        return actions;
    }

    private async Task<List<ActionInfo>> LoadActions(FunctionType functionType)
    {
        var allActions = new List<ActionInfo>();
        // tenant actions with action source system
        allActions.AddRange(await TenantService.GetSystemScriptActionsAsync<ActionInfo>(TenantId, functionType));
        // payroll actions with action source script
        allActions.AddRange(await PayrollService.GetActionsAsync<ActionInfo>(new(TenantId, PayrollId),
            functionType: functionType));
        return allActions;
    }
}