﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class DivisionBackendService : BackendServiceBase<DivisionService, TenantServiceContext, ViewModel.Division, Query>
{
    public DivisionBackendService(UserSession userSession, IConfiguration configuration) :
        base(userSession, configuration)
    {
    }

    /// <summary>The current request context</summary>
    protected override TenantServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        UserSession.Tenant != null ? new TenantServiceContext(UserSession.Tenant.Id) : null;

    /// <summary>Create the backend service</summary>
    protected override DivisionService CreateService(IDictionary<string, object> parameters = null) =>
        new(HttpClient);
}