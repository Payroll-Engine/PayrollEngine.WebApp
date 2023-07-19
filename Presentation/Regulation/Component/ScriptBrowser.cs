using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
using Task = System.Threading.Tasks.Task;
using Tenant = PayrollEngine.Client.Model.Tenant;

namespace PayrollEngine.WebApp.Presentation.Regulation.Component;

internal class ScriptBrowser : ItemBrowserBase
{
    internal ScriptBrowser(Tenant tenant, Payroll payroll, List<Client.Model.Regulation> regulations,
        IPayrollService payrollService, IScriptService scriptService) :
        base(tenant, payroll, regulations, payrollService)
    {
        ScriptService = scriptService ?? throw new ArgumentNullException(nameof(scriptService));
    }

    private ItemCollection<RegulationScript> scripts;
    private ScriptFactory scriptFactory;
    private IScriptService ScriptService { get; }

    internal override void Reset()
    {
        scripts = null;
        scriptFactory = null;
    }

    internal ItemCollection<RegulationScript> Scripts => scripts ??= LoadScripts();
    private ScriptFactory ScriptFactory => scriptFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, ScriptService);
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await ScriptFactory.SaveItem(scripts, item as RegulationScript);

    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await ScriptFactory.DeleteItem(scripts, item as RegulationScript);

    protected override void OnDispose() =>
        scripts?.Dispose();

    private ItemCollection<RegulationScript> LoadScripts() =>
        new(Task.Run(ScriptFactory.LoadPayrollItems).Result);
}