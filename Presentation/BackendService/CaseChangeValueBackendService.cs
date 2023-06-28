using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class CaseChangeValueBackendService : BackendServiceBase<PayrollCaseChangeValueService, PayrollServiceContext, ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery>
{
    public CaseChangeValueBackendService(UserSession userSession, IConfiguration configuration) :
        base(userSession, configuration)
    {
    }

    protected override bool CanRead(IDictionary<string, object> parameters = null)
    {
        return base.CanRead(parameters) && UserSession.Payroll != null;
    }

    /// <summary>The current request context</summary>
    protected override void SetupReadQuery(PayrollServiceContext context, PayrollCaseChangeQuery query, IDictionary<string, object> parameters = null)
    {
        query.UserId = UserSession.User.Id;
        query.Culture = UserSession.User.Culture;

        // division filter
        query.DivisionId = UserSession.Payroll.DivisionId;
        // set cluster filter if available
        var clusterSetFilter = UserSession.Payroll.ClusterSetCase;
        if (!string.IsNullOrWhiteSpace(clusterSetFilter))
        {
            query.ClusterSetName = clusterSetFilter;
        }
    }

    /// <summary>The current request context</summary>
    protected override PayrollServiceContext CreateServiceContext(IDictionary<string, object> parameters = null)
    {
        if (UserSession.Tenant == null || UserSession.Payroll == null)
        {
            return null;
        }
        return new(UserSession.Tenant.Id, UserSession.Payroll.Id);
    }

    /// <summary>Create the backend service</summary>
    protected override PayrollCaseChangeValueService CreateService(IDictionary<string, object> parameters = null) => 
        new(HttpClient);

    /// <summary>Localize slots</summary>
    protected override void ProcessReceivedItems(ViewModel.CaseChangeCaseValue[] resultItems, IDictionary<string, object> parameters = null) =>
        resultItems.LocalizeSlot(UserSession.User.Culture);
}