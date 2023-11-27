using System.Collections.Generic;
using System.Net.Http;
using PayrollEngine.Client;
using PayrollEngine.Client.Model;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public abstract class CaseChangeValueBackendServiceBase(UserSession userSession, HttpClientHandler httpClientHandler,
        PayrollHttpConfiguration configuration, Localizer localizer)
    : BackendServiceBase<PayrollCaseChangeValueService, PayrollServiceContext, ViewModel.CaseChangeCaseValue, PayrollCaseChangeQuery>(userSession, httpClientHandler, configuration, localizer)
{
    protected override bool CanRead()
    {
        return base.CanRead() && UserSession.Payroll != null;
    }

    /// <summary>The current request context</summary>
    protected override void SetupReadQuery( PayrollCaseChangeQuery query, IDictionary<string, object> parameters = null)
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
    protected override PayrollCaseChangeValueService CreateService() => 
        new(HttpClient);

    /// <summary>Localize slots</summary>
    protected override void ProcessReceivedItems(ViewModel.CaseChangeCaseValue[] resultItems) =>
        resultItems.LocalizeSlot(UserSession.User.Culture);
}