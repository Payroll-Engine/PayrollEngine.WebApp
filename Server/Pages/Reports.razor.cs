using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation.Report;
using PayrollEngine.WebApp.Server.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Pages;

public partial class Reports : IReportOperator
{
    public Reports() :
        base(WorkingItems.TenantChange | WorkingItems.PayrollChange)
    {
    }

    [Inject]
    protected IPayrollService PayrollService { get; set; }
    [Inject]
    protected IReportService ReportService { get; set; }
    [Inject]
    protected IReportSetService ReportSetService { get; set; }

    /// <inheritdoc />
    protected override async Task OnTenantChangedAsync(Client.Model.Tenant tenant)
    {
        await base.OnTenantChangedAsync(tenant);
        await SetupReportsAsync();
    }

    /// <inheritdoc />
    protected override async Task OnPayrollChangedAsync(Client.Model.Payroll payroll)
    {
        await base.OnPayrollChangedAsync(payroll);
        await SetupReportsAsync();
    }

    #region Grid

    /// <summary>
    /// The grid column configuration
    /// </summary>
    protected List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GetTenantGridId(GridIdentifiers.Reports));

    #endregion

    #region Report

    protected List<Report> ReportSets { get; set; } = new();

    private async Task SetupReportsAsync()
    {
        if (!HasPayroll)
        {
            return;
        }

        // retrieve active payroll reports
        var reports = await PayrollService.GetReportsAsync<Report>(
            new(Tenant.Id, Payroll.Id));
        ReportSets = reports;
        StateHasChanged();
    }

    #endregion

    #region Actions

    public async Task ShowReportLogAsync(Report report)
    {
        var parameters = new DialogParameters
        {
            { nameof(ReportLogsDialog.Tenant), Tenant },
            { nameof(ReportLogsDialog.Report), report },
            { nameof(ReportLogsDialog.ValueFormatter), ValueFormatter }
        };
        await DialogService.ShowAsync<ReportLogsDialog>("Report Logs", parameters);
    }

    public async Task StartReportAsync(Report report)
    {
        // report parameters
        var parameters = new DialogParameters
        {
            { nameof(ReportDownloadDialog.Tenant), Tenant },
            { nameof(ReportDownloadDialog.User), User },
            { nameof(ReportDownloadDialog.Payroll), Payroll },
            { nameof(ReportDownloadDialog.Report), report },
            { nameof(ReportDownloadDialog.Language), UserLanguage },
            { nameof(ReportDownloadDialog.ValueFormatter), ValueFormatter }
        };
        await DialogService.ShowAsync<ReportDownloadDialog>(
            "Report Download", parameters);
    }

    #endregion

    #region Lifecycle

    protected override async Task OnInitializedAsync()
    {
        await SetupReportsAsync();
        await base.OnInitializedAsync();
    }

    #endregion

}