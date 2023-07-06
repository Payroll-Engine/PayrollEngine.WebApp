using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component
{
    internal abstract class ItemBrowserBase : IDisposable
    {
        protected Client.Model.Tenant Tenant { get; }
        protected Client.Model.Payroll Payroll { get; }
        protected List<Client.Model.Regulation> Regulations { get; }
        protected IPayrollService PayrollService { get; }

        protected ItemBrowserBase(Client.Model.Tenant tenant, Client.Model.Payroll payroll,
            List<Client.Model.Regulation> regulations, IPayrollService payrollService)
        {
            Tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
            Payroll = payroll ?? throw new ArgumentNullException(nameof(payroll));
            Regulations = regulations ?? throw new ArgumentNullException(nameof(regulations));
            PayrollService = payrollService ?? throw new ArgumentNullException(nameof(payrollService));
        }

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
}
