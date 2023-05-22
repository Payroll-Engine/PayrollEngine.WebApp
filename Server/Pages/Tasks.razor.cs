using System;
using System.Linq;
using System.Threading.Tasks;
using PayrollEngine.Client.Service;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Server.Dialogs;
using PayrollEngine.WebApp.Server.Shared;
using Task = System.Threading.Tasks.Task;
using Blazored.LocalStorage;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Tasks : ITaskOperator<ViewModel.Task>
{
    // injected services
    [Inject]
    protected ITaskService TaskService { get; set; }
    [Inject]
    protected TaskBackendService TaskBackendService { get; set; }
    [Inject]
    public ILocalStorageService LocalStorage { get; set; }

    protected override string GridId => GetTenantGridId(GridIdentifiers.Tasks);
    protected override IBackendService<ViewModel.Task, Query> BackendService => TaskBackendService;
    protected override ItemCollection<ViewModel.Task> Items { get; } = new();

    public Tasks() :
        base(WorkingItems.TenantChange)
    {
    }

    #region Item

    protected override async Task<bool> OnItemCommit(ViewModel.Task payroll, ItemOperation operation)
    {
        SetupTask(payroll);
        return await base.OnItemCommit(payroll, operation);
    }

    private void SetupTask(ViewModel.Task task)
    {
        var dateUpdated = false;

        // add current time to date object (user can't select time)
        var now = DateTime.Now;
        var currentTimeSpan = new TimeSpan(now.Hour, now.Minute, now.Second);

        if (task.Scheduled.HasValue)
        {
            var scheduled = task.Scheduled.Value;
            if (scheduled.IsMidnight())
            {
                dateUpdated = true;
                task.Scheduled = scheduled.Date.Add(currentTimeSpan);
            }
        }

        var completed = task.Completed;
        if (completed != null && !completed.HasTime())
        {
            dateUpdated = true;
            task.Completed = completed.Value.Date.Add(currentTimeSpan);
        }
        // if dates changed, update user
        if (dateUpdated)
        {
            task.ScheduledUserId = User.Id;
        }
    }

    public virtual async Task CompleteItemAsync(ViewModel.Task task)
    {
        var parameters = new DialogParameters
        {
            { nameof(ViewModel.Task), task }
        };
        var result = await (await DialogService.ShowAsync<TaskCompleteDialog>("Complete Task", parameters)).Result;
        if (result == null || result.Canceled)
        {
            return;
        }

        try
        {
            task.Completed = DateTime.Now;
            await TaskService.UpdateAsync(new(Tenant.Id), task);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorAsync(exception);
        }
    }

    #endregion

    #region Filter

    protected bool CompletedTasks { get; set; }
    private FilterDefinition<ViewModel.Task> completedFilter;

    private async Task ToggleTasksFilterAsync()
    {
        CompletedTasks = !CompletedTasks;
        if (CompletedTasks)
        {
            // show all
            await ItemsGrid.ClearFiltersAsync();
        }
        else
        {
            // apply completed filter
            await ItemsGrid.AddFilterAsync(completedFilter);
        }

        // store theme setting
        await LocalStorage.SetItemAsBooleanAsync("TasksCompleted", CompletedTasks);
    }

    private async Task InitFilterAsync()
    {
        // completed tasks filter
        var column = ItemsGrid.RenderedColumns.FirstOrDefault(
            x => string.Equals(x.PropertyName, nameof(ViewModel.Task.Completed)));
        if (column == null)
        {
            return;
        }
        completedFilter = new()
        {
            Column = column,
            Operator = "is empty"
        };

        // completed tasks filter
        var completedTasks = await LocalStorage.GetItemAsBooleanAsync("TasksCompleted");
        if (completedTasks.HasValue)
        {
            CompletedTasks = completedTasks.Value;
            StateHasChanged();
        }

        // apply completed filter
        if (!CompletedTasks)
        {
            await ItemsGrid.AddFilterAsync(completedFilter);
        }
    }

    #endregion

    #region Lifecycle

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await InitFilterAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion

}