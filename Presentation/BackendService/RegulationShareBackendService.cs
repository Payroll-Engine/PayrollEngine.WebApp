﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using PayrollEngine.Client.Service;
using PayrollEngine.Client.Service.Api;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public class RegulationShareBackendService : BackendServiceBase<RegulationShareService, RootServiceContext, RegulationShare, Query>
{
    private ITenantService TenantService { get; }
    private IRegulationService RegulationService { get; }
    private IDivisionService DivisionService { get; }

    public RegulationShareBackendService(UserSession userSession, IConfiguration configuration, Localizer localizer,
        ITenantService tenantService, IRegulationService regulationService, IDivisionService divisionService) :
        base(userSession, configuration, localizer)
    {
        TenantService = tenantService ?? throw new ArgumentNullException(nameof(tenantService));
        RegulationService = regulationService ?? throw new ArgumentNullException(nameof(regulationService));
        DivisionService = divisionService ?? throw new ArgumentNullException(nameof(divisionService));
    }

    /// <summary>The current request context</summary>
    protected override RootServiceContext CreateServiceContext(IDictionary<string, object> parameters = null) =>
        new();

    /// <summary>Create the backend service</summary>
    protected override RegulationShareService CreateService() =>
        new(HttpClient);


    public async Task ApplyShareAsync(RegulationShare share, IDictionary<string, object> parameters = null) =>
        await ApplyShareAsync(new[] { share }, parameters);

    private async Task ApplyShareAsync(IEnumerable<RegulationShare> shares, IDictionary<string, object> parameters = null)
    {
        // culture
        string culture = null;
        if (parameters != null && parameters.TryGetValue("Culture", out var cultureValue))
        {
            culture = (string)cultureValue;
        }

        var tenants = await TenantService.QueryAsync<Tenant>(new(), new Query { Status = ObjectStatus.Active });
        foreach (var share in shares)
        {
            // provider tenant
            var providerTenant = tenants.FirstOrDefault(x => x.Id == share.ProviderTenantId);
            if (providerTenant == null)
            {
                continue;
            }
            share.ProviderTenantIdentifier = providerTenant.Identifier;

            // provider regulation
            var providerRegulation = await RegulationService.GetAsync<ViewModel.Regulation>(new(providerTenant.Id), share.ProviderRegulationId);
            if (providerRegulation == null)
            {
                continue;
            }
            share.ProviderRegulationName = culture != null ?
                culture.GetLocalization(providerRegulation.NameLocalizations, providerRegulation.Name) :
                providerRegulation.Name;

            // consumer tenant
            var consumerTenant = tenants.FirstOrDefault(x => x.Id == share.ConsumerTenantId);
            if (consumerTenant == null)
            {
                continue;
            }
            share.ConsumerTenantIdentifier = consumerTenant.Identifier;

            // consumer division
            if (share.ConsumerDivisionId.HasValue)
            {
                var consumerDivision = await DivisionService.GetAsync<Division>(new(share.ConsumerTenantId), share.ConsumerDivisionId.Value);
                if (consumerDivision == null)
                {
                    continue;
                }
                share.ConsumerDivisionName = culture != null ?
                    culture.GetLocalization(consumerDivision.NameLocalizations, consumerDivision.Name) :
                    consumerDivision.Name;
            }
        }
    }

    protected override async Task OnItemsReadAsync(List<RegulationShare> shares)
    {
        await ApplyShareAsync(shares);
        await base.OnItemsReadAsync(shares);
    }
}