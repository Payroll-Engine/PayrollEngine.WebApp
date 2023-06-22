using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using NPOI.XSSF.UserModel;
using PayrollEngine.Client.Service;
using PayrollEngine.Data;
using PayrollEngine.Document;
using PayrollEngine.IO;
using PayrollEngine.WebApp.Presentation;
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
    private IPayrollService PayrollService { get; set; }
    [Inject]
    private IJSRuntime JsRuntime { get; set; }

    /// <inheritdoc />
    protected override async Task OnTenantChangedAsync(Client.Model.Tenant tenant)
    {
        await SetupReportsAsync();
        SetupAvailableReports();
        await base.OnTenantChangedAsync(tenant);
    }

    /// <inheritdoc />
    protected override async Task OnPayrollChangedAsync(Client.Model.Payroll payroll)
    {
        await SetupReportsAsync();
        SetupAvailableReports();
        await base.OnPayrollChangedAsync(payroll);
    }

    #region Grid

    private MudDataGrid<Report> ReportsGrid { get; set; }

    /// <summary>
    /// The grid column configuration
    /// </summary>
    protected List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GetTenantGridId(GridIdentifiers.Reports));

    #endregion

    #region Clusters

    private const string ClusterAll = "All";

    /// <summary>
    /// The filtered/working clusters
    /// </summary>
    public List<string> Clusters { get; set; }

    /// <summary>
    /// Test for filtered/working clusters
    /// </summary>
    public bool HasClusters => Clusters != null && Clusters.Any();

    /// <summary>
    /// The selected cluster
    /// </summary>
    public MudChip SelectedCluster { get; set; }

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
    private void SelectedClusterChanged(MudChip cluster)
    {
        SetupAvailableReports(cluster?.Text);
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

    protected List<Report> AllReports { get; set; } = new();
    protected List<Report> AvailableReports { get; set; } = new();

    private async Task SetupReportsAsync()
    {
        if (!HasPayroll)
        {
            return;
        }

        // reset report clusters
        ResetClusters();

        // retrieve active payroll reports
        var reports = await PayrollService.GetReportsAsync<Report>(
            new(Tenant.Id, Payroll.Id));
        AllReports = reports;

        // cluster setup
        SetupClusters();
    }

    private void SetupAvailableReports() =>
        SetupAvailableReports(SelectedCluster?.Text);

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
    protected async Task ResetFilterAsync() =>
        await ReportsGrid.ClearFiltersAsync();

    /// <summary>
    /// Download excel file from unfiltered grid data
    /// <remarks>Copy from <see cref="ItemPageBase{TItem,TQuery}.ExcelDownloadAsync"/> </remarks>
    /// </summary>
    protected async Task ExcelDownloadAsync()
    {
        // retrieve all items, without any filter and sort
        if (!AvailableReports.Any())
        {
            await UserNotification.ShowErrorMessageBoxAsync("Excel Download", "Empty collection");
            return;
        }

        try
        {
            // column properties
            var properties = ReportsGrid.GetColumnProperties();
            if (!properties.Any())
            {
                return;
            }

            // convert items to data set
            var name = "PayrunResults";
            var dataSet = new System.Data.DataSet(name);
            var dataTable = AvailableReports.ToSystemDataTable(name, includeRows: true, properties: properties);
            dataSet.Tables.Add(dataTable);

            // xlsx workbook
            using var workbook = new XSSFWorkbook();
            // import 
            workbook.Import(dataSet);

            // result
            using var resultStream = new MemoryStream();
            workbook.Write(resultStream, true);
            resultStream.Seek(0, SeekOrigin.Begin);

            var download = $"{name}_{FileTool.CurrentTimeStamp()}{FileExtensions.ExcelDocument}";
            await JsRuntime.SaveAs(download, resultStream.ToArray());
            await UserNotification.ShowSuccessAsync("Download completed");
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync("Excel download error", exception);
        }
    }

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
        SetupAvailableReports();
        await base.OnInitializedAsync();
    }

    #endregion

}