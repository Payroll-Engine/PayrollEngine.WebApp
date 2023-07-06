using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component
{
    internal class CaseBrowser : ItemBrowserBase
    {
        internal CaseBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
            IPayrollService payrollService, ICaseService caseService) :
            base(tenant, payroll, regulations, payrollService)
        {
            CaseService = caseService ?? throw new ArgumentNullException(nameof(caseService));
        }

        private ItemCollection<RegulationCase> cases;
        private CaseFactory caseFactory;
        private ICaseService CaseService { get; }

        internal override void Reset()
        {
            cases = null;
            caseFactory = null;
        }

        internal ItemCollection<RegulationCase> Cases => cases ??= LoadCases();
        internal CaseFactory CaseFactory => caseFactory ??=
            new(Tenant, Payroll, Regulations, PayrollService, CaseService);
        internal override async Task<bool> SaveAsync(IRegulationItem item) =>
            await CaseFactory.SaveItem(cases, item as RegulationCase);

        internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
            await CaseFactory.DeleteItem(cases, item as RegulationCase);

        protected override void OnDispose() =>
            cases?.Dispose();

        private ItemCollection<RegulationCase> LoadCases() =>
            new(Task.Run(CaseFactory.LoadPayrollItems).Result);
    }
}
