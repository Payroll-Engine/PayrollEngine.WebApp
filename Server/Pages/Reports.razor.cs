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

    private MudDataGrid<Report> ReportsGrid { get; set; }

    /// <summary>
    /// The grid column configuration
    /// </summary>
    protected List<GridColumnConfiguration> ColumnConfiguration =>
        GetColumnConfiguration(GetTenantGridId(GridIdentifiers.Reports));

    #endregion

    #region Report

    protected List<Report> Items { get; set; } = new();

    private async Task SetupReportsAsync()
    {
        if (!HasPayroll)
        {
            return;
        }

        // retrieve active payroll reports
        var reports = await PayrollService.GetReportsAsync<Report>(
            new(Tenant.Id, Payroll.Id));
        Items = reports;
        StateHasChanged();
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
        if (!Items.Any())
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
            var dataTable = Items.ToSystemDataTable(name, includeRows: true, properties: properties);
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
        await base.OnInitializedAsync();
    }

    #endregion

}