using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.BackendService;
using PayrollEngine.WebApp.Presentation.Payrun;
using PayrollEngine.WebApp.Server.Shared;
using Employee = PayrollEngine.WebApp.ViewModel.Employee;
using Payrun = PayrollEngine.WebApp.ViewModel.Payrun;
using PayrunJob = PayrollEngine.WebApp.ViewModel.PayrunJob;
using PayrunParameter = PayrollEngine.WebApp.ViewModel.PayrunParameter;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class PayrunJobs : IPayrunJobOperator
{
    [Parameter]
    public string Payrun { get; set; }

    [Inject]
    private PayrunPayrunJobBackendService PayrunPayrunJobBackendService { get; set; }
    [Inject]
    private IPayrunParameterService PayrunParameterService { get; set; }
    [Inject]
    private IPayrunJobService PayrunJobService { get; set; }
    [Inject]
    private IPayrunService PayrunService { get; set; }
    [Inject]
    private IEmployeeService EmployeeService { get; set; }
    [Inject]
    private IUserService UserService { get; set; }

    public PayrunJobs() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    /// <inheritdoc />
    protected override async Task OnTenantChangedAsync()
    {
        // update users on tenant change
        await SetupUsersAsync();
        await SetupPage();
        await base.OnTenantChangedAsync();
        StateHasChanged();
    }

    /// <inheritdoc />
    protected override async Task OnPayrollChangedAsync(Client.Model.Payroll payroll)
    {
        await SetupPage();
        await base.OnPayrollChangedAsync(payroll);
        StateHasChanged();
    }

    #region Legal Payrun Jobs

    private MudForm legalForm;
    private MudDataGrid<PayrunJob> LegalJobsGrid { get; set; }

    /// <summary>
    /// The working legal payrun job
    /// </summary>
    private PayrunJob LegalJob { get; set; }

    /// <summary>
    /// The legal payrun job setup
    /// </summary>
    private PayrunJobSetup LegalSetup { get; } = new();

    /// <summary>
    /// True if legal payrun job is available
    /// </summary>
    private bool HasLegalJob => LegalJob != null;

    /// <summary>
    /// The grid column configuration
    /// </summary>
    private List<GridColumnConfiguration> LegalColumnConfiguration =>
        GetColumnConfiguration(GetTenantGridId(GridIdentifiers.PayrunLegalJobs));

    /// <summary>
    /// Start a legal payrun job
    /// </summary>
    private async Task StartLegalJobAsync()
    {
        if (LegalJob != null)
        {
            throw new InvalidOperationException();
        }

        // validation
        if (!await legalForm.Revalidate())
        {
            await UserNotification.ShowErrorAsync(Localizer.PayrunJob.JobValidationFailed);
            return;
        }

        // start job
        if (await StartJobAsync(LegalSetup))
        {
            // refresh legal payrun job
            await SetupLegalJobAsync();
            // refresh legal data
            await RefreshLegalServerDataAsync();
            // refresh forecast data too
            await RefreshForecastServerDataAsync();
        }
    }

    /// <summary>
    /// Release a legal payrun job
    /// </summary>
    private async Task ReleaseLegalJobAsync()
    {
        if (LegalJob == null || LegalJob.JobStatus != PayrunJobStatus.Draft)
        {
            return;
        }
        await ChangeLegalStatusAsync(PayrunJobStatus.Release);
    }

    /// <summary>
    /// Process a legal payrun job
    /// </summary>
    private async Task ProcessLegalJobAsync()
    {
        if (SelectedPayrun == null || LegalJob.JobStatus != PayrunJobStatus.Release)
        {
            return;
        }
        await ChangeLegalStatusAsync(PayrunJobStatus.Process);
    }

    /// <summary>
    /// Complete a legal payrun job
    /// </summary>
    private async Task CompleteLegalJobAsync()
    {
        if (LegalJob == null || LegalJob.JobStatus != PayrunJobStatus.Process)
        {
            return;
        }
        await ChangeLegalStatusAsync(PayrunJobStatus.Complete);
    }

    /// <summary>
    /// Cancel a legal payrun job
    /// </summary>
    private async Task CancelLegalJobAsync()
    {
        if (LegalJob == null || LegalJob.JobStatus != PayrunJobStatus.Process)
        {
            return;
        }
        await ChangeLegalStatusAsync(PayrunJobStatus.Cancel);
    }

    /// <summary>
    /// Abort a legal payrun job
    /// </summary>
    private async Task AbortLegalJobAsync()
    {
        if (LegalJob == null ||
            (LegalJob.JobStatus != PayrunJobStatus.Draft &&
             LegalJob.JobStatus != PayrunJobStatus.Release))
        {
            return;
        }
        await ChangeLegalStatusAsync(PayrunJobStatus.Abort);
    }

    /// <summary>
    /// Change the legal payrun job status
    /// </summary>
    /// <param name="jobStatus">The target status</param>
    private async Task ChangeLegalStatusAsync(PayrunJobStatus jobStatus)
    {
        if (LegalJob == null)
        {
            return;
        }

        // confirmation
        if (!await DialogService.ShowMessageBoxAsync(
                Localizer.PayrunJob.PayrunJob,
                new MarkupString($"<br /><b>&#xbb;{jobStatus}&#xab;</b> {Localizer.PayrunJob.PayrunJob} {LegalJob.Name}?<br /><br />"),
                Localizer.Dialog.Ok,
                Localizer.Dialog.Cancel))
        {
            return;
        }

        try
        {
            var jobId = LegalJob.Id;
            // change the payrun job status
            await PayrunJobService.ChangeJobStatusAsync(new(Tenant.Id), jobId,
                jobStatus, User.Id, Localizer.PayrunJob.DefaultReason(jobStatus), true);

            // refresh legal payrun job
            await SetupLegalJobAsync();

            // refresh data
            await RefreshLegalServerDataAsync();

            await UserNotification.ShowSuccessAsync(Localizer.PayrunJob.StatusChanged(jobStatus));
            StateHasChanged();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunJob.PayrunJob, exception);
        }
    }

    /// <summary>
    /// Setup legal payrun job
    /// </summary>
    private async Task SetupLegalJobAsync()
    {
        // reset
        if (SelectedPayrun == null)
        {
            LegalJob = null;
            await ResetJobSetupAsync(LegalSetup);
            return;
        }

        try
        {
            // retrieve the latest payrun job from the selected payrun, ignoring the forecast jobs
            var query = new Query
            {
                Filter = new Equals(nameof(PayrunJob.PayrollId), Payroll.Id)
                    .And(new Equals(nameof(PayrunJob.PayrunId), SelectedPayrun.Id)
                        .And(new NotEquals(nameof(PayrunJob.JobStatus), PayrunJobStatus.Forecast.ToString()))),
                OrderBy = new OrderBy(nameof(PayrunJob.Updated), OrderDirection.Descending),
                Top = 1
            };
            var legalJob =
                (await PayrunJobService.QueryAsync<PayrunJob>(new(Tenant.Id), query)).FirstOrDefault();

            // exclude payrun jobs which are in final state
            if (legalJob != null && legalJob.JobStatus.IsFinal())
            {
                legalJob = null;
            }

            if (legalJob == null)
            {
                LegalJob = null;
                await ResetJobSetupAsync(LegalSetup);
                return;
            }

            LegalJob = legalJob;
            await ApplyJobSetupAsync(LegalSetup, legalJob);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            if (Initialized)
            {
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunJob.PayrunJob, exception);
            }
            else
            {
                await UserNotification.ShowErrorAsync(exception, exception.GetBaseMessage());
            }
        }
    }

    /// <summary>
    /// Get legal payrun server data, handler for data grids
    /// </summary>
    /// <param name="state">The data grid state</param>
    /// <returns>Collection of items</returns>
    private async Task<GridData<PayrunJob>> GetLegalServerDataAsync(GridState<PayrunJob> state)
    {
        try
        {
            if (SelectedPayrun == null)
            {
                throw new InvalidOperationException("Please ensure selected payrun");
            }

            // forecast job filter
            var forecastColumn = nameof(PayrunJob.JobStatus);
            var forecastValue = nameof(PayrunJobStatus.Forecast);
            var column = LegalJobsGrid.RenderedColumns.FirstOrDefault(
                x => string.Equals(x.PropertyName, forecastColumn));
            if (column != null)
            {
                // remove existing
                var forecastFilter = state.FilterDefinitions.FirstOrDefault(
                    x => string.Equals(x.Column?.PropertyName, forecastColumn) &&
                         string.Equals(x.Value?.ToString(), forecastValue));
                if (forecastFilter != null)
                {
                    await UserNotification.ShowWarningAsync(Localizer.Forecast.JobNotSupported);
                    state.FilterDefinitions.Remove(forecastFilter);
                }

                // exclude forecast payrun jobs
                state.FilterDefinitions.Add(new FilterDefinition<PayrunJob>
                {
                    Column = column,
                    Operator = "is not",
                    Value = forecastValue,
                    Title = forecastValue
                });
            }

            // server request parameters
            Dictionary<string, object> parameters = new() {
                { nameof(PayrunJob.PayrunId), SelectedPayrun.Id }
            };
            return await PayrunPayrunJobBackendService.QueryAsync(state, parameters: parameters);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            return new();
        }
    }

    /// <summary>
    /// Refresh legal payrun job
    /// </summary>
    private async Task RefreshLegalServerDataAsync()
    {
        if (LegalJobsGrid == null)
        {
            return;
        }
        await LegalJobsGrid.ReloadServerData();
    }

    #endregion

    #region Forecast Payrun Jobs

    private MudForm forecastForm;
    private MudDataGrid<PayrunJob> ForecastJobsGrid { get; set; }

    /// <summary>
    /// The forecast payrun job setup
    /// </summary>
    private PayrunJobSetup ForecastSetup { get; } = new();

    /// <summary>
    /// The grid column configuration
    /// </summary>
    private List<GridColumnConfiguration> ForecastColumnConfiguration =>
        GetColumnConfiguration(GetTenantGridId(GridIdentifiers.PayrunForecastJobs));

    /// <summary>
    /// Start a forecast payrun job
    /// </summary>
    private async Task StartForecastJobAsync()
    {
        if (string.IsNullOrWhiteSpace(ForecastSetup.Reason))
        {
            throw new InvalidOperationException();
        }

        // validation
        if (!await forecastForm.Revalidate())
        {
            await UserNotification.ShowErrorAsync(Localizer.PayrunJob.JobValidationFailed);
            return;
        }

        // start job
        if (await StartJobAsync(ForecastSetup))
        {
            // refresh forecast payrun job
            await SetupForecastJobAsync();
            // refresh data
            await RefreshForecastServerDataAsync();
        }
    }

    /// <summary>
    /// Copy payrun job data to the working forecast setup
    /// </summary>
    /// <param name="payrunJob">The copy source</param>
    async Task IPayrunJobOperator.CopyForecastJobAsync(PayrunJob payrunJob)
    {
        // confirmation
        var result = await DialogService.ShowMessageBoxAsync(
            Localizer.Forecast.Copy,
            Localizer.Forecast.CopyQuery,
            Localizer.PayrunJob.Copy,
            Localizer.Dialog.Cancel);
        if (!result)
        {
            return;
        }
        await ApplyJobSetupAsync(ForecastSetup, payrunJob);
        StateHasChanged();
    }

    /// <summary>
    /// Setup forecast payrun job
    /// </summary>
    private async Task SetupForecastJobAsync()
    {
        // reset
        if (SelectedPayrun == null)
        {
            await ResetJobSetupAsync(ForecastSetup);
            return;
        }

        try
        {
            // retrieve latest forecast payrun job
            var query = new Query
            {
                Filter = new Equals(nameof(PayrunJob.PayrollId), Payroll.Id)
                    .And(new Equals(nameof(PayrunJob.PayrunId), SelectedPayrun.Id)
                        .And(new Equals(nameof(PayrunJob.JobStatus), PayrunJobStatus.Forecast.ToString()))),
                OrderBy = new OrderBy(nameof(PayrunJob.Updated), OrderDirection.Descending),
                Top = 1
            };
            var forecastJob =
                (await PayrunJobService.QueryAsync<PayrunJob>(new(Tenant.Id), query)).FirstOrDefault();

            if (forecastJob == null)
            {
                await ResetJobSetupAsync(ForecastSetup);
                return;
            }

            await ApplyJobSetupAsync(ForecastSetup, forecastJob);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            if (Initialized)
            {
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunJob.PayrunJob, exception);
            }
            else
            {
                await UserNotification.ShowErrorAsync(exception, exception.GetBaseMessage());
            }
        }
    }

    /// <summary>
    /// Get forecast payrun server data, handler for data grids
    /// </summary>
    /// <param name="state">The data grid state</param>
    /// <returns>Collection of items</returns>
    private async Task<GridData<PayrunJob>> GetForecastServerDataAsync(GridState<PayrunJob> state)
    {
        try
        {
            if (SelectedPayrun == null)
            {
                throw new InvalidOperationException("Please ensure selected payrun");
            }

            // server request parameters
            Dictionary<string, object> parameters = new() {
                { nameof(PayrunJob.PayrunId), SelectedPayrun.Id }
            };
            return await PayrunPayrunJobBackendService.QueryAsync(state, parameters: parameters);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            return new();
        }
    }

    /// <summary>
    /// Refresh the forecast payrun jobs
    /// </summary>
    /// <returns></returns>
    private async Task RefreshForecastServerDataAsync()
    {
        if (ForecastJobsGrid == null)
        {
            return;
        }
        await ForecastJobsGrid.ReloadServerData();
    }

    #endregion

    #region Payrun Jobs

    /// <summary>
    /// Start payrun job
    /// </summary>
    /// <param name="setup">The payrun job setup</param>
    /// <returns>True if job was started successfully</returns>
    private async Task<bool> StartJobAsync(PayrunJobSetup setup)
    {
        var title = string.IsNullOrWhiteSpace(setup.ForecastName) ?
            Localizer.PayrunJob.StartPayrun : Localizer.Forecast.StartForecastPayrun;

        if (!setup.Period.HasValue)
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, title, Localizer.PayrunJob.MissingJobPeriod);
            Log.Error("Missing job period");
            return false;
        }
        if (string.IsNullOrWhiteSpace(setup.Reason))
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, title, Localizer.PayrunJob.MissingJobReason);
            Log.Error("Missing job reason");
            return false;
        }

        var employees = setup.SelectedEmployees?.ToList() ?? Employees;
        var parameters = new DialogParameters
        {
            { nameof(PayrunStartDialog.Tenant), Tenant },
            { nameof(PayrunStartDialog.User), User },
            { nameof(PayrunStartDialog.Employees), employees },
            { nameof(PayrunStartDialog.Payroll), Payroll},
            { nameof(PayrunStartDialog.PayrunId), SelectedPayrun.Id },
            { nameof(PayrunStartDialog.Setup) , setup }
        };
        var result = await (await DialogService.ShowAsync<PayrunStartDialog>(title, parameters)).Result;
        if (result.Canceled)
        {
            return false;
        }

        // jump to results
        if (result.Data is bool viewResults && viewResults)
        {
            NavigateToResults();
            return false;
        }
        return true;
    }

    /// <summary>
    /// Show payrun job details dialog
    /// </summary>
    /// <param name="payrunJob">The payrun job to show</param>
    async Task IPayrunJobOperator.ShowJobAsync(PayrunJob payrunJob)
    {
        // payrun job dialog (read only)
        var parameters = new DialogParameters
        {
            { nameof(PayrunJobDialog.Tenant), Tenant },
            { nameof(PayrunJobDialog.Culture), PageCulture },
            { nameof(PayrunJobDialog.PayrunJob), payrunJob },
            { nameof(PayrunJobDialog.Users), Users },
            { nameof(PayrunJobDialog.Employees), Employees }
        };
        await DialogService.ShowAsync<PayrunJobDialog>(
            title: payrunJob.JobStatus == PayrunJobStatus.Forecast ?
                $"{Localizer.Forecast.Forecast} {Localizer.PayrunJob.PayrunJob}" : Localizer.PayrunJob.PayrunJob,
            parameters);
    }

    /// <summary>
    /// Reset payrun job the reset
    /// </summary>
    /// <param name="setup">The payrun job setup</param>
    private async Task ResetJobSetupAsync(PayrunJobSetup setup)
    {
        var now = Date.Now;
        var period = new DateTime(now.Year, now.Month, 1).AddMonths(1);
        setup.Period = period;
        setup.JobName = $"{Localizer.Payrun.Payrun} {Date.GetMonthName(period.Month)} {period.Year}";
        setup.ForecastName = null;
        setup.SelectedEmployees = null;

        // job reason
        setup.Reason = null;
        if (SelectedPayrun != null)
        {
            setup.Reason = SelectedPayrun.DefaultReason;
            setup.Parameters = await PayrunParameterService.QueryAsync<PayrunParameter>(
                new(Tenant.Id, SelectedPayrun.Id));
        }
    }

    /// <summary>
    /// Apply payrun job to the payrun job setup
    /// </summary>
    /// <param name="setup">The jo setup</param>
    /// <param name="job">The payrun job to apply</param>
    private async Task ApplyJobSetupAsync(PayrunJobSetup setup, PayrunJob job)
    {
        setup.Period = job.PeriodStart;
        setup.JobName = job.Name;
        setup.Reason = job.CreatedReason;
        setup.ForecastName = job.Forecast;
        setup.Reason = job.CreatedReason;
        // employee selection
        setup.SelectedEmployees = job.Employees.Any() ?
            Employees.Where(x => job.Employees.Any(y => y.Id == x.Id)).ToList() :
            null;
        setup.Parameters = await PayrunParameterService.QueryAsync<PayrunParameter>(
                new(Tenant.Id, job.PayrunId));
    }

    #endregion

    #region Job Results

    private void NavigateToResults()
    {
        if (SelectedPayrun == null)
        {
            return;
        }
        var jobResultsUrl = $"{PageUrls.PayrunResults}/{SelectedPayrun.Name}";
        NavigateTo(jobResultsUrl);
    }

    #endregion

    #region Employees

    /// <summary>
    /// All employees from the payroll division
    /// </summary>
    private List<Employee> Employees { get; set; }

    /// <summary>
    /// Setup payroll employees
    /// </summary>
    private async Task SetupEmployeesAsync()
    {
        try
        {
            var employees = await EmployeeService.QueryAsync<Employee>(new(Tenant.Id),
                new()
                {
                    Status = ObjectStatus.Active,
                    DivisionId = Payroll.DivisionId
                });
            // employees ordered by full name
            Employees = employees.OrderBy(x => x.FullName).ToList();
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            if (Initialized)
            {
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunJob.PayrunJob, exception);
            }
            else
            {
                await UserNotification.ShowErrorAsync(exception, exception.GetBaseMessage());
            }
        }
    }

    #endregion

    #region Payruns

    /// <summary>
    /// The payruns of the working payroll
    /// </summary>
    private List<Payrun> Payruns { get; set; }

    /// <summary>
    /// True if payroll contains payruns
    /// </summary>
    private bool HasPayruns => Payruns != null && Payruns.Any();

    /// <summary>
    /// Selected payrun
    /// </summary>
    private Payrun SelectedPayrun { get; set; }

    /// <summary>
    /// Selected payrun by name
    /// </summary>
    protected string SelectedPayrunName
    {
        get => SelectedPayrun?.Name;
        set => throw new NotSupportedException();
    }

    /// <summary>
    /// Setup payruns of the working payroll
    /// </summary>
    /// <returns></returns>
    private async Task SetupPayrunsAsync()
    {
        try
        {
            // retrieve payruns by payroll
            Query query = new()
            {
                Filter = new Equals(nameof(ViewModel.Payrun.PayrollId), Payroll.Id)
            };
            var payruns =
                HasPayroll ?
                await PayrunService.QueryAsync<Payrun>(new(Tenant.Id), query) :
                null;
            if (payruns == null)
            {
                Payruns = null;
                SelectedPayrun = null;
                return;
            }

            // payrun collection
            Payruns = payruns.Select(x => new Payrun(x)).ToList();

            // selected payrun
            Payrun selected = null;
            // page parameter on startup
            if (!Initialized && !string.IsNullOrWhiteSpace(Payrun))
            {
                selected = Payruns.FirstOrDefault(x =>
                    string.Equals(x.Name, Payrun, StringComparison.InvariantCultureIgnoreCase));
            }
            // payroll with single payrun
            if (selected == null && payruns.Count == 1)
            {
                selected = payruns.First();
            }
            SelectedPayrun = selected;
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            if (Initialized)
            {
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunJob.PayrunJob, exception);
            }
            else
            {
                await UserNotification.ShowErrorAsync(exception, exception.GetBaseMessage());
            }
        }
    }

    /// <summary>
    /// Payrun changed handler
    /// </summary>
    /// <param name="payrunName">The new payrun</param>
    private async Task PayrunChanged(string payrunName)
    {
        if (string.Equals(payrunName, SelectedPayrunName))
        {
            return;
        }
        var payrun = Payruns?.FirstOrDefault(x => string.Equals(x.Name, payrunName));
        if (payrun == null)
        {
            await UserNotification.ShowErrorAsync(Localizer.Payrun.UnknownPayrun(payrunName));
            return;
        }

        // payrun selection
        SelectedPayrun = payrun;

        // payrun job setup
        await SetupPayrunJobsAsync();

        // refresh payrun job grid
        if (Initialized)
        {
            await RefreshLegalServerDataAsync();
            await RefreshForecastServerDataAsync();
        }
    }

    /// <summary>
    /// Start the payrun parameter dialog
    /// </summary>
    /// <param name="payrunParameters">The payrun parameters to setup</param>
    private async Task SetupPayrunParametersAsync(List<PayrunParameter> payrunParameters)
    {
        // setup payrun parameters
        foreach (var payrunParameter in payrunParameters)
        {
            payrunParameter.ValueFormatter = ValueFormatter;
        }

        var dialogParameters = new DialogParameters
        {
            { nameof(PayrunParameterDialog.Parameters), payrunParameters }
        };
        var result = await (await DialogService.ShowAsync<PayrunParameterDialog>(
            Localizer.PayrunParameter.PayrunParameters, dialogParameters)).Result;
        if (!result.Canceled)
        {
            StateHasChanged();
        }
    }

    #endregion

    #region Users

    /// <summary>
    /// The users of the working tenant
    /// </summary>
    private List<User> Users { get; set; }

    /// <summary>
    /// Get user full name
    /// </summary>
    /// <param name="userId">The user id</param>
    /// <returns>The user name</returns>
    private string GetUserName(int? userId)
    {
        if (!userId.HasValue)
        {
            return null;
        }

        var user = Users.FirstOrDefault(x => x.Id == userId);
        if (user == null)
        {
            return null;
        }
        return $"{user.FirstName} {user.LastName}";
    }

    /// <summary>
    /// Setup users of the working tenant
    /// </summary>
    private async Task SetupUsersAsync()
    {
        try
        {
            var users = await UserService.QueryAsync<User>(new(Tenant.Id),
                new() { Status = ObjectStatus.Active });
            Users = users;
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            if (Initialized)
            {
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.PayrunJob.PayrunJob, exception);
            }
            else
            {
                await UserNotification.ShowErrorAsync(exception, exception.GetBaseMessage());
            }
        }
    }

    #endregion

    #region Lifecycle

    /// <summary>
    /// Setup legal and forecast job setup after a payrun change
    /// </summary>
    private async Task SetupPayrunJobsAsync()
    {
        if (!HasPayroll)
        {
            return;
        }
        await SetupLegalJobAsync();
        await SetupForecastJobAsync();
    }

    /// <summary>
    /// Setup page data after a tenant or payroll change
    /// </summary>
    private async Task SetupPage()
    {
        if (!HasPayroll)
        {
            return;
        }
        await SetupEmployeesAsync();
        await SetupPayrunsAsync();
        await SetupPayrunJobsAsync();
    }

    private bool Initialized { get; set; }
    protected override async Task OnInitializedAsync()
    {
        await SetupUsersAsync();
        await SetupPage();
        await base.OnInitializedAsync();
        Initialized = true;
    }

    #endregion

}
