using System.Collections.Generic;

namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// Regulation edit context
/// </summary>
public class RegulationEditContext
{
    /// <summary>
    /// The regulation tenant
    /// </summary>
    public Client.Model.Tenant Tenant { get; init; }

    /// <summary>
    /// The system user
    /// </summary>
    public Client.Model.User User { get; init; }

    /// <summary>
    /// The regulation payroll
    /// </summary>
    public Client.Model.Payroll Payroll { get; init; }

    /// <summary>
    /// The payroll regulations
    /// </summary>
    public List<Client.Model.Regulation> Regulations { get; init; }

    /// <summary>
    /// The action provider
    /// </summary>
    public ActionProvider ActionProvider { get; init; }
}