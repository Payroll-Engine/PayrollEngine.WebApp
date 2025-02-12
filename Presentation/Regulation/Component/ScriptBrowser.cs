using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Regulation.Factory;
using PayrollEngine.WebApp.ViewModel;
using Payroll = PayrollEngine.Client.Model.Payroll;
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
    private IScriptService ScriptService { get; }
    private ScriptFactory scriptFactory;
    private ScriptFactory ScriptFactory => scriptFactory ??=
        new(Tenant, Payroll, Regulations, PayrollService, ScriptService);

    internal ItemCollection<RegulationScript> Scripts => 
        scripts ??= LoadScripts();
    internal override async Task<bool> SaveAsync(IRegulationItem item) =>
        await ScriptFactory.SaveItem(scripts, item as RegulationScript);
    internal override async Task<IRegulationItem> DeleteAsync(IRegulationItem item) =>
        await ScriptFactory.DeleteItem(scripts, item as RegulationScript);

    protected override void OnContextChanged()
    {
        scripts = null;
        scriptFactory = null;
    }

    protected override void OnDispose() =>
        scripts?.Dispose();

    private ItemCollection<RegulationScript> LoadScripts()
    {
        var items = System.Threading.Tasks.Task.Run(ScriptFactory.LoadPayrollItemsAsync).Result;
        return new ItemCollection<RegulationScript>(items);
    }
}