﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Tenants() : EditItemPageBase<Tenant, Query, Dialogs.TenantDialog>(
    WorkingItems.None, useCalendar: true)
{
    [Inject]
    private TenantBackendService TenantBackendService { get; set; }
    [Inject]
    private PayrollHttpClient PayrollHttpClient { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Tenants);
    protected override IBackendService<Tenant, Query> BackendService => TenantBackendService;
    protected override ItemCollection<Tenant> Items => Session.Tenants;
    protected override bool AddItemTenantParameter => false;

    /// <summary>
    /// Ensure valid editing tenant context.
    /// </summary>
    protected override Client.Model.Tenant GetItemTenant(ItemOperation operation, Tenant item) =>
        operation == ItemOperation.Edit ? item : Tenant;

    protected override string GetLocalizedItemName(bool plural) =>
        plural ? Localizer.Tenant.Tenants : Localizer.Tenant.Tenant;

    /// <summary>
    /// Disable tenant authentication on new tenant
    /// </summary>
    public override async Task CreateItemAsync()
    {
        var tenant = Tenant;
        try
        {
            // disable tenant authorization
            PayrollHttpClient.RemoveTenantAuthorization();

            // base class handling
            await base.CreateItemAsync();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
        }
        finally
        {
            if (tenant != null)
            {
                // restore tenant authorization
                PayrollHttpClient.SetTenantAuthorization(tenant.Identifier);
            }
        }
    }

    /// <summary>
    /// Change tenant authentication to the editing item
    /// </summary>
    /// <param name="item"></param>
    public override async Task EditItemAsync(Tenant item)
    {
        var tenant = Tenant;
        try
        {
            // change tenant authorization
            PayrollHttpClient.SetTenantAuthorization(item.Identifier);

            // base class handling
            await base.EditItemAsync(item);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
        }
        finally
        {
            if (tenant != null)
            {
                // restore tenant authorization
                PayrollHttpClient.SetTenantAuthorization(tenant.Identifier);
            }
        }
    }

    public override async Task DeleteItemAsync(Tenant item)
    {
        var tenant = Tenant;
        try
        {
            // disable tenant authorization
            PayrollHttpClient.RemoveTenantAuthorization();

            // base class handling
            await base.DeleteItemAsync(item);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
        }
        finally
        {
            if (tenant != null)
            {
                // restore tenant authorization
                PayrollHttpClient.SetTenantAuthorization(tenant.Identifier);
            }
        }
    }

    /// <inheritdoc />
    protected override async Task<bool> SetupDialogParametersAsync(DialogParameters parameters, ItemOperation operation, Tenant item)
    {
        if (operation == ItemOperation.Edit)
        {
            parameters.Add(nameof(Tenant), item);
        }
        return await base.SetupDialogParametersAsync(parameters, operation, item);
    }

}