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
    protected Client.Model.Tenant Tenant { get; private set; } = tenant ?? throw new ArgumentNullException(nameof(tenant));
    protected Client.Model.Payroll Payroll { get; private set; } = payroll ?? throw new ArgumentNullException(nameof(payroll));
    protected List<Client.Model.Regulation> Regulations { get; private set; } = regulations ?? throw new ArgumentNullException(nameof(regulations));
    protected IPayrollService PayrollService { get; } = payrollService ?? throw new ArgumentNullException(nameof(payrollService));

    internal void ChangeContext(RegulationEditContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        Tenant = context.Tenant;
        Payroll = context.Payroll;
        Regulations = context.Regulations;
        OnContextChanged();
    }
    internal abstract Task<bool> SaveAsync(IRegulationItem item);
    internal abstract Task<IRegulationItem> DeleteAsync(IRegulationItem item);

    protected abstract void OnContextChanged();
    protected abstract void OnDispose();

    public void Dispose()
    {
        OnDispose();
        GC.SuppressFinalize(this);
    }
}