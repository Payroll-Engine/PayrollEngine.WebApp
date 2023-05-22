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
    public Client.Model.Tenant Tenant { get; set; }

    /// <summary>
    /// The system user
    /// </summary>
    public Client.Model.User User { get; set; }

    /// <summary>
    /// The regulation payroll
    /// </summary>
    public Client.Model.Payroll Payroll { get; set; }

    /// <summary>
    /// The payroll regulations
    /// </summary>
    public List<Client.Model.Regulation> Regulations { get; set; }

    /// <summary>
    /// The action provider
    /// </summary>
    public ActionProvider ActionProvider { get; set; }
}