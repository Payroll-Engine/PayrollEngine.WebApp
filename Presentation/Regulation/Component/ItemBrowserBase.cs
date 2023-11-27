using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal abstract class ItemBrowserBase(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
        List<Client.Model.Regulation> regulations, IPayrollService payrollService)
    : IDisposable
{
    protected Client.Model.Tenant Tenant { get; } = tenant ?? throw new ArgumentNullException(nameof(tenant));
    protected Client.Model.Payroll Payroll { get; } = payroll ?? throw new ArgumentNullException(nameof(payroll));
    protected List<Client.Model.Regulation> Regulations { get; } = regulations ?? throw new ArgumentNullException(nameof(regulations));
    protected IPayrollService PayrollService { get; } = payrollService ?? throw new ArgumentNullException(nameof(payrollService));

    internal abstract void Reset();
    internal abstract Task<bool> SaveAsync(IRegulationItem item);
    internal abstract Task<IRegulationItem> DeleteAsync(IRegulationItem item);

    public void Dispose()
    {
        OnDispose();
        GC.SuppressFinalize(this);
    }

    protected abstract void OnDispose();
}