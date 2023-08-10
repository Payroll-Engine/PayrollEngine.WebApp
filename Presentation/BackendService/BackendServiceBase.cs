using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation.BackendService;

public abstract class BackendServiceBase<TService, TServiceContext, TItem, TQuery> : IBackendService<TItem, TQuery>, IDisposable
    where TService : IReadService<TItem, TServiceContext, TQuery>
    where TServiceContext : IServiceContext
    where TItem : class, new()
    where TQuery : Query, new()
{
    private readonly QueryBuilder<TQuery, TItem> queryBuilder = new();
    protected UserSession UserSession { get; }
    private Localizer Localizer { get; }
    private TService Service { get; set; }

    /// <summary>The http client</summary>
    protected PayrollHttpClient HttpClient { get; }

    private readonly string ItemTypeName = typeof(TItem).Name.ToPascalSentence();

    protected BackendServiceBase(UserSession userSession, IConfiguration configuration, Localizer localizer)
    {
        UserSession = userSession ?? throw new ArgumentNullException(nameof(userSession));
        Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));

        // http configuration
        var httpConfiguration = Task.Run(configuration.GetHttpConfigurationAsync).Result;
        if (httpConfiguration == null)
        {
            throw new PayrollException("Missing payroll http configuration");
        }

        // http connection
        HttpClient = new(httpConfiguration);
        UpdateAuthorization();
    }

    /// <inheritdoc />
    public int? MaximumItemCount
    {
        get => queryBuilder.MaximumItemCount;
        set => queryBuilder.MaximumItemCount = value;
    }

    #region Service

    // the current request context
    protected abstract TServiceContext CreateServiceContext(IDictionary<string, object> parameters = null);

    /// <summary>Create the api service</summary>
    protected abstract TService CreateService();

    /// <summary>Test for valid read state</summary>
    protected virtual bool CanRead() => true;

    /// <summary>Setup the api service query</summary>
    protected virtual void SetupReadQuery(TQuery query, IDictionary<string, object> parameters = null)
    {
    }

    protected virtual void ProcessReceivedItems(TItem[] resultItems)
    {
    }

    #endregion

    #region Query and Get

    /// <inheritdoc />
    public virtual async Task<GridData<TItem>> QueryAsync(GridState<TItem> state,
        IQueryResolver resolver = null, IDictionary<string, object> parameters = null)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        var query = queryBuilder.BuildQuery(state, resolver);
        return await QueryAsync(query, parameters);
    }

    /// <inheritdoc />
    public virtual async Task<GridData<TItem>> QueryAsync(TQuery query = null, IDictionary<string, object> parameters = null)
    {
        query ??= new();
        UpdateAuthorization();

        Service = CreateService();
        if (Service == null)
        {
            throw new PayrollException("Missing payroll service");
        }

        // test for available read state
        if (!CanRead())
        {
            return new();
        }

        // query items
        try
        {
            LogStopwatch.Start(nameof(QueryAsync));

            var context = CreateServiceContext(parameters);
            if (context == null)
            {
                // request without context not possible
                return new();
            }
            SetupReadQuery(query, parameters);

            Log.Trace($"Query to {typeof(TService).Name}: {query}");
            var result = await Service.QueryResultAsync<TItem>(context, query);

            LogStopwatch.Stop(nameof(QueryAsync));

            // initialize result object
            var gridData = new GridData<TItem>
            {
                TotalItems = (int)result.Count
            };

            if (result.Items == null || result.Items.Length == 0)
            {
                gridData.Items = Enumerable.Empty<TItem>();
                return gridData;
            }

            // give option to setup result item before any other operation
            ProcessReceivedItems(result.Items);

            Log.Trace($"response from {typeof(TService).Name}: {result.Items.Length} of {result.Count}");

            var itemsList = result.Items.ToList();
            gridData.Items = itemsList;

            // derived notification
            await OnItemsReadAsync(itemsList);

            return gridData;
        }
        catch (Exception exception)
        {
            Log.Error(exception.GetBaseMessage(), exception);
            await UserSession.UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.ItemRead(ItemTypeName), exception);
            return new();
        }
    }

    /// <inheritdoc />
    public virtual async Task<TItem> GetAsync(int itemId)
    {
        try
        {
            var readService = Service as IReadService<TItem, TServiceContext, TQuery>;
            if (readService == null)
            {
                throw new PayrollException($"Service {typeof(TService).Name} can't create the type {ItemTypeName}");
            }
            Log.Trace($"Read object on {typeof(TService).Name} with id: {itemId}");
            var item = await readService.GetAsync<TItem>(CreateServiceContext(), itemId);

            // derived notification
            await OnItemsReadAsync(new() { item });

            return item;
        }
        catch (Exception exception)
        {
            Log.Error(exception.GetBaseMessage(), exception);
            await UserSession.UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.ItemRead(ItemTypeName), exception);
            return null;
        }
    }

    protected virtual Task OnItemsReadAsync(List<TItem> payrunJobs) =>
        Task.CompletedTask;

    #endregion

    #region Create

    /// <inheritdoc />
    public virtual async Task<TItem> CreateAsync(TItem item)
    {
        try
        {
            UpdateAuthorization();

            var createService = Service as ICreateService<TItem, TServiceContext, TQuery>;
            if (createService == null)
            {
                throw new PayrollException($"Service {typeof(TService).Name} can't create the type {ItemTypeName}");
            }
            Log.Trace($"Create object on {typeof(TService).Name}: {item}");
            return await createService.CreateAsync(CreateServiceContext(), item);
        }
        catch (Exception exception)
        {
            Log.Error(exception.GetBaseMessage(), exception);
            await UserSession.UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.ItemCreate(ItemTypeName), exception);
            return null;
        }
    }

    #endregion

    #region Update

    /// <inheritdoc />
    public virtual async Task<TItem> UpdateAsync(TItem item)
    {
        try
        {
            if (item == null)
            {
                return null;
            }

            UpdateAuthorization();

            var crudService = Service as ICrudService<TItem, TServiceContext, TQuery>;
            if (crudService == null)
            {
                throw new PayrollException($"Service {typeof(TService).Name} can't update the type {ItemTypeName}");
            }
            Log.Trace($"Update object on {typeof(TService).Name}: {item}");
            await crudService.UpdateAsync(CreateServiceContext(), item);
            return item;
        }
        catch (Exception exception)
        {
            Log.Error(exception.GetBaseMessage(), exception);
            await UserSession.UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.ItemUpdate(ItemTypeName), exception);
            return null;
        }
    }

    #endregion

    #region Delete

    /// <inheritdoc />
    public virtual async Task<bool> DeleteAsync(int itemId)
    {
        try
        {
            if (itemId <= 0)
            {
                return false;
            }

            UpdateAuthorization();

            var createService = Service as ICreateService<TItem, TServiceContext, TQuery>;
            if (createService == null)
            {
                throw new PayrollException($"Service {typeof(TService).Name} can't delete the type {ItemTypeName}");
            }
            Log.Trace($"Delete object on {typeof(TService).Name}: {itemId}");
            await createService.DeleteAsync(CreateServiceContext(), itemId);
            return true;
        }
        catch (Exception exception)
        {
            Log.Error(exception.GetBaseMessage(), exception);
            await UserSession.UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Error.ItemDelete(ItemTypeName), exception);
            return false;
        }
    }

    #endregion

    private void UpdateAuthorization() =>
        HttpClient.SetTenantAuthorization(UserSession.Tenant.Identifier);

    public virtual void Dispose()
    {
        HttpClient?.Dispose();
    }
}