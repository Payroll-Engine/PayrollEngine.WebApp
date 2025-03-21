﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.Presentation;
using PayrollEngine.WebApp.Presentation.Component;
using PayrollEngine.WebApp.Presentation.Report;
using PayrollEngine.WebApp.Server.Components.Shared;
using PayrollEngine.WebApp.ViewModel;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Server.Components.Pages;

public partial class Reports() : PageBase(WorkingItems.TenantChange | WorkingItems.PayrollChange), IReportOperator
{
    [Inject]
    private IPayrollService PayrollService { get; set; }

    /// <inheritdoc />
    protected override async Task OnTenantChangedAsync()
    {
        await SetupReportsAsync();
        SetupAvailableReports();
        await base.OnTenantChangedAsync();
    }

    /// <inheritdoc />
    protected override async Task OnPayrollChangedAsync(Client.Model.Payroll payroll)
    {
        await SetupReportsAsync();
        SetupAvailableReports();
        await base.OnPayrollChangedAsync(payroll);
    }

    #region Grid

    private MudDataGrid<ReportSet> ReportsGrid { get; set; }

    /// <summary>
    /// The grid column configuration
    /// </summary>
    private List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GetTenantGridId(GridIdentifiers.Reports));

    #endregion

    #region Clusters

    private string ClusterAll => Localizer.Shared.All;

    /// <summary>
    /// The filtered/working clusters
    /// </summary>
    private List<string> Clusters { get; set; }

    /// <summary>
    /// Test for filtered/working clusters
    /// </summary>
    private bool HasClusters => Clusters != null && Clusters.Any();

    /// <summary>
    /// The selected cluster
    /// </summary>
    private string SelectedCluster { get; set; }

    /// <summary>
    /// Setup filtered/working clusters
    /// </summary>
    private void SetupClusters()
    {
        var uniqueClusters = new HashSet<string>();

        // collect report clusters
        if (AllReports != null)
        {
            foreach (var report in AllReports)
            {
                if (report.Clusters != null)
                {
                    foreach (var cluster in report.Clusters)
                    {
                        uniqueClusters.Add(cluster);
                    }
                }
            }
        }
        var clusters = uniqueClusters.ToList();
        // cluster all
        if (clusters.Any())
        {
            clusters.Insert(0, ClusterAll);
        }
        Clusters = clusters;
    }

    /// <summary>
    /// Handler for cluster change
    /// </summary>
    /// <param name="cluster">The selected cluster</param>
    private void SelectedClusterChanged(string cluster)
    {
        SetupAvailableReports(cluster);
        SelectedCluster = cluster;
    }

    /// <summary>
    /// Reset all clusters
    /// </summary>
    private void ResetClusters()
    {
        Clusters = null;
        SelectedCluster = null;
    }

    #endregion

    #region Report

    private List<ReportSet> AllReports { get; set; } = [];
    private List<ReportSet> AvailableReports { get; set; } = [];

    private async Task SetupReportsAsync()
    {
        if (!HasPayroll)
        {
            return;
        }

        // reset report clusters
        ResetClusters();

        // retrieve active payroll reports
        var reports = await PayrollService.GetReportsAsync<ReportSet>(
            new(Tenant.Id, Payroll.Id),
            userType: User.UserType);
        AllReports = reports;

        // cluster setup
        SetupClusters();
    }

    private void SetupAvailableReports() =>
        SetupAvailableReports(SelectedCluster);

    private void SetupAvailableReports(string cluster)
    {
        AvailableReports = null;
        if (AllReports == null)
        {
            return;
        }

        // all available reports: no cluster or all
        if (string.IsNullOrWhiteSpace(cluster) || string.Equals(cluster, ClusterAll))
        {
            AvailableReports = AllReports;
            return;
        }

        // filter available reports by cluster
        AvailableReports = AllReports.
                Where(x => x.Clusters != null && x.Clusters.Contains(cluster)).ToList();
    }

    #endregion

    #region Actions

    /// <summary>
    /// Reset all grid filters
    /// </summary>
    private async Task ResetFilterAsync() =>
        await ReportsGrid.ClearFiltersAsync();

    /// <summary>
    /// Download excel file from unfiltered grid data
    /// <remarks>Copy from <see cref="ItemPageBase{TItem,TQuery}.ExcelDownloadAsync"/> </remarks>
    /// </summary>
    private async Task ExcelDownloadAsync()
    {
        // retrieve all items, without any filter and sort
        if (!AvailableReports.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report,
                Localizer.Error.EmptyCollection);
            return;
        }

        try
        {
            await ExcelDownload.StartAsync(ReportsGrid, AvailableReports, JsRuntime, Localizer.Report.Reports);
            await UserNotification.ShowSuccessAsync(Localizer.Shared.DownloadCompleted);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report, exception);
        }
    }

    public async Task ShowReportLogAsync(ReportSet report)
    {
        var parameters = new DialogParameters
        {
            { nameof(ReportLogsDialog.Tenant), Tenant },
            { nameof(ReportLogsDialog.Report), report },
            { nameof(ReportLogsDialog.ValueFormatter), ValueFormatter }
        };
        await DialogService.ShowAsync<ReportLogsDialog>(Localizer.ReportLog.ReportLogs, parameters);
    }

    public async Task BuildReportAsync(ReportSet report)
    {
        // report parameters
        var parameters = new DialogParameters
        {
            { nameof(ReportBuildDialog.Tenant), Tenant },
            { nameof(ReportBuildDialog.Culture), PageCulture },
            { nameof(ReportBuildDialog.User), User },
            { nameof(ReportBuildDialog.Payroll), Payroll },
            { nameof(ReportBuildDialog.Report), report },
            { nameof(ReportBuildDialog.ValueFormatter), ValueFormatter }
        };
        await DialogService.ShowAsync<ReportBuildDialog>(
            report.GetLocalizedName(PageCulture.Name), parameters);
    }

    #endregion

    #region Lifecycle

    protected override async Task OnPageInitializedAsync()
    {
        await SetupReportsAsync();
        SetupAvailableReports();
        await base.OnPageInitializedAsync();
    }

    #endregion

}