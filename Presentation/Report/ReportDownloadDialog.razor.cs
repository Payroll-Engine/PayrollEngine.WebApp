using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using PayrollEngine.Client;
using PayrollEngine.Client.Service;
using PayrollEngine.Data;
using PayrollEngine.Document;
using PayrollEngine.IO;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.WebApp.ViewModel;
using DataSet = System.Data.DataSet;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Report;

public partial class ReportDownloadDialog
{
    private MudForm form;

    [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
    [Parameter] public Tenant Tenant { get; set; }
    [Parameter] public User User { get; set; }
    [Parameter] public Client.Model.Payroll Payroll { get; set; }
    [Parameter] public ReportSet Report { get; set; }
    [Parameter] public string Culture { get; set; }
    [Parameter] public ValueFormatter ValueFormatter { get; set; }

    [Inject]
    private IPayrollService PayrollService { get; set; }
    [Inject]
    private IReportService ReportService { get; set; }
    [Inject]
    private IReportSetService ReportSetService { get; set; }
    [Inject]
    private IDataMerge DataMerge { get; set; }
    [Inject]
    private IUserNotificationService UserNotification { get; set; }
    [Inject]
    private IJSRuntime JsRuntime { get; set; }
    [Inject]
    private Localizer Localizer { get; set; }

    private Client.Model.ReportTemplate ReportTemplate { get; set; }

    private bool Executing { get; set; }
    private bool Completed { get; set; }
    private bool Failed { get; set; }
    private bool Started => Executing || Completed || Failed;

    private string ErrorMessage { get; set; }
    private string DownloadFileName { get; set; }

    private string ReportName => Report.GetLocalizedName(Culture);

    private string ReportDescription => Report.GetLocalizedDescription(Culture);
    private bool HasDescription =>
        !string.IsNullOrWhiteSpace(ReportDescription);

    private bool HasVisibleParameters() =>
        Report.ViewParameters == null ||
        Report.ViewParameters.Count(x => x.Attributes?.GetHidden(Culture) ?? false) != Report.ViewParameters.Count;

    private bool SupportedDocumentType(DocumentType documentType) =>
        DataMerge.IsMergeable(documentType);

    private void Close()
    {
        if (Completed)
        {
            MudDialog.Close(DialogResult.Ok(true));
        }
        MudDialog.Close(DialogResult.Cancel());
    }

    /// <summary>
    /// Start the report generation
    /// </summary>
    /// <param name="documentType">The target document type</param>
    private async Task StartAsync(DocumentType documentType)
    {
        // form validation
        if (form != null && !await form.Revalidate())
        {
            return;
        }

        // ensure report template
        if (documentType is DocumentType.Word or DocumentType.Pdf or DocumentType.Xml && ReportTemplate == null)
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report,
                Localizer.Report.TemplateNotAvailable(ReportName, Culture));
            return;
        }

        try
        {
            Executing = true;
            StateHasChanged();

            // generate parameter dictionary
            var parameters = Report.ViewParameters.ToDictionary(p => p.Name, p => p.Value);

            var response = await ReportService.ExecuteReportAsync(
                new(Tenant.Id, Report.RegulationId), Report.Id,
                new()
                {
                    Culture = Culture,
                    Parameters = parameters,
                    UserId = User.Id
                });

            // report metadata
            var now = DateTime.Now; // use local time (no UTC)
            var title = Report.GetLocalizedName(Culture);
            var documentMetadata = new DocumentMetadata
            {
                Author = User.Identifier,
                Category = Report.Category,
                Company = Tenant.Identifier,
                Title = title,
                Keywords = response.Culture,
                CustomProperties = parameters,
                Created = now,
                Modified = now
            };

            // data set
            DataSet dataSet = response.Result.ToSystemDataSet();
            if (!dataSet.HasRows())
            {
                Failed = true;
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report, Localizer.Report.EmptyReport);
            }
            else
            {
                // download
                DownloadFileName =
                    $"{Report.Name}_{FileTool.CurrentTimeStamp()}{documentType.GetFileExtension()}";
                var reportName = Report.GetLocalizedName(Culture);

                var mergeParameters = new Dictionary<string,object>(parameters.Select(x => new KeyValuePair<string, object>(x.Key, x.Value)));

                // document stream
                MemoryStream documentStream = null;
                switch (documentType)
                {
                    case DocumentType.Excel:
                        documentStream = DataMerge.ExcelMerge(dataSet, documentMetadata, mergeParameters);
                        break;
                    case DocumentType.Word:
                    case DocumentType.Pdf:
                        documentStream = DataMerge.Merge(
                            new MemoryStream(Convert.FromBase64String(ReportTemplate.Content)),
                            dataSet, documentType, documentMetadata, mergeParameters);
                        break;
                    case DocumentType.Xml:
                        var transformedXml = XmlUtil.TransformXmlFromXsl(dataSet, ReportTemplate.Content);
                        if (string.IsNullOrWhiteSpace(ReportTemplate.Schema) ||
                            XmlUtil.ValidateXmlString(transformedXml, ReportTemplate.Schema))
                        {
                            documentStream = XmlUtil.XmlToMemoryStream(transformedXml);
                        }
                        else
                        {
                            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report,
                                Localizer.Report.XmlValidationError(reportName));
                        }
                        break;
                    case DocumentType.XmlRaw:
                        var rawXml = XmlUtil.TransformXml(dataSet);
                        if (!string.IsNullOrWhiteSpace(rawXml))
                        {
                            documentStream = XmlUtil.XmlToMemoryStream(rawXml);
                        }
                        else
                        {
                            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report,
                                Localizer.Report.EmptyXmlRaw(reportName));
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(documentType));
                }

                // browser download
                if (documentStream != null)
                {
                    await JsRuntime.SaveAs(DownloadFileName, documentStream.ToArray());
                }
            }

            Executing = false;
            Completed = true;
            StateHasChanged();
        }
        catch (Exception exception)
        {
            ErrorMessage = exception.GetApiErrorMessage();
            Executing = false;
            Failed = true;
            StateHasChanged();
        }
    }

    /// <summary>
    /// Build or rebuild the report including the report parameters
    /// </summary>
    /// <param name="report">The target report</param>
    private async Task UpdateReportAsync(ReportSet report = null)
    {
        if (!Initialized)
        {
            return;
        }

        // check if set is loaded for first time to avoid rendering problems
        ReportSet reportSet;
        if (report != null)
        {
            reportSet = new(report);
            ApplySystemParameters(reportSet);
        }
        else
        {
            reportSet = Report;
        }

        // prepare request
        var reportRequest = new Client.Model.ReportRequest
        {
            UserId = User.Id,
            Culture = User.Culture
        };
        if (reportSet.ViewParameters != null)
        {
            reportRequest.Parameters = reportSet.ViewParameters.ToDictionary(p => p.Name, p => p.Value);
        }

        // update report
        try
        {
            // retrieve report with updated parameters
            var buildReport = await ReportSetService.GetAsync<ReportSet>(
                new(Tenant.Id, reportSet.RegulationId), reportSet.Id, reportRequest);
            // convert to view model
            var buildReportSet = new ReportSet(buildReport);

            // clear previous report handler
            RemoveReportHandler(Report);

            // replace report
            Report = buildReportSet;

            // register new report handler
            AddReportHandler(buildReportSet);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report, exception);
            return;
        }

        // report template
        await SetupReportTemplateAsync();
    }

    /// <summary>
    /// Setup the report template
    /// </summary>
    private async Task SetupReportTemplateAsync()
    {
        if (Report == null)
        {
            return;
        }

        try
        {
            // template by culture
            ReportTemplate = (await PayrollService.GetReportTemplatesAsync<Client.Model.ReportTemplate>(
                new(Tenant.Id, Payroll.Id),
                reportNames: new[] { Report.Name },
                culture: Culture)).FirstOrDefault();

            // fallback template by base culture
            if (ReportTemplate == null)
            {
                var index = Culture.IndexOf('-');
                if (index >= 0)
                {
                    var baseCulture = Culture.Substring(0, index);
                    ReportTemplate = (await PayrollService.GetReportTemplatesAsync<Client.Model.ReportTemplate>(
                        new(Tenant.Id, Payroll.Id),
                        reportNames: new[] { Report.Name },
                        culture: baseCulture)).FirstOrDefault();
                }
            }

        }
        catch (HttpRequestException exception)
        {
            if (exception.StatusCode != HttpStatusCode.NotFound)
            {
                Log.Error(exception, exception.GetBaseMessage());
                await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report, exception);
            }
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report, exception);
        }

        // template test
        if (ReportTemplate == null)
        {
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report,
                Localizer.Error.UnknownItem(Localizer.ReportTemplate.ReportTemplate, Culture));
        }
    }

    private static void ApplySystemParameters(ReportSet reportSet)
    {
        foreach (var parameter in reportSet.ViewParameters)
        {
            switch (parameter.ParameterType)
            {
                // date
                case ReportParameterType.Now:
                    parameter.ValueAsDateTime = DateTime.Now;
                    break;
                case ReportParameterType.Today:
                    parameter.ValueAsDateTime = DateTime.Today;
                    break;
            }
        }
    }

    private void UpdateReportParameter(ReportParameter parameter) =>
        OnParameterChangedAsync();

    private async void OnParameterChangedAsync()
    {
        await UpdateReportAsync();
        StateHasChanged();
    }

    private void AddReportHandler(ReportSet report)
    {
        if (report == null)
        {
            return;
        }

        foreach (var parameter in report.ViewParameters)
        {
            // value formatter
            parameter.ValueFormatter = ValueFormatter;
            // change notification
            parameter.ParameterChanged += UpdateReportParameter;
        }
    }

    private void RemoveReportHandler(ReportSet report)
    {
        if (report == null)
        {
            return;
        }

        foreach (var parameter in report.ViewParameters)
        {
            // value formatter
            parameter.ValueFormatter = null;
            // change notification
            parameter.ParameterChanged -= UpdateReportParameter;
        }
    }

    private bool Initialized { get; set; }
    protected override async Task OnInitializedAsync()
    {
        AddReportHandler(Report);
        ApplySystemParameters(Report);
        await SetupReportTemplateAsync();
        await base.OnInitializedAsync();
        
        Initialized = true;
        // build initial report parameters
        await UpdateReportAsync(Report);
    }
}