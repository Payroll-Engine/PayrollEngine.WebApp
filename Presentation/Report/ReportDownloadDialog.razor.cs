using System;
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
using PayrollEngine.WebApp.ViewModel;
using DataSet = System.Data.DataSet;
using Task = System.Threading.Tasks.Task;

namespace PayrollEngine.WebApp.Presentation.Report
{
    public partial class ReportDownloadDialog
    {
        private MudForm form;

        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }
        [Parameter] public Tenant Tenant { get; set; }
        [Parameter] public User User { get; set; }
        [Parameter] public Client.Model.Payroll Payroll { get; set; }
        [Parameter] public ReportSet Report { get; set; }
        [Parameter] public Language Language { get; set; }
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

        private Client.Model.ReportTemplate ReportTemplate { get; set; }

        private bool Executing { get; set; }
        private bool Completed { get; set; }
        private bool Failed { get; set; }
        private bool Started => Executing || Completed || Failed;

        private string ErrorMessage { get; set; }
        private string DownloadFileName { get; set; }

        private string ReportName => Report.GetLocalizedName(Language);

        private string ReportDescription => Report.GetLocalizedDescription(Language);
        private bool HasDescription =>
            !string.IsNullOrWhiteSpace(ReportDescription);

        private bool HasParameters =>
            Report.Parameters != null && Report.Parameters.Any();

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
            if (!await form.Revalidate())
            {
                return;
            }

            try
            {
                Executing = true;
                StateHasChanged();

                // generate parameter dictionary
                var parameters = Report.Parameters.ToDictionary(p => p.Name, p => p.Value);

                var response = await ReportService.ExecuteReportAsync(
                    new(Tenant.Id, Report.RegulationId), Report.Id,
                    new()
                    {
                        Language = Language,
                        Parameters = parameters,
                        UserId = User.Id
                    });

                // report metadata
                var now = DateTime.Now; // use local time (no UTC)
                var title = Report.GetLocalizedName(Language);
                var documentMetadata = new DocumentMetadata
                {
                    Author = User.Identifier,
                    Category = Report.Category,
                    Company = Tenant.Identifier,
                    Title = title,
                    Keywords = response.Language.ToString(),
                    CustomProperties = parameters,
                    Created = now,
                    Modified = now
                };

                // data set
                DataSet dataSet = response.Result.ToSystemDataSet();
                if (!dataSet.HasRows())
                {
                    Failed = true;
                    await UserNotification.ShowErrorMessageBoxAsync("Report start", "Report without data");
                }
                else
                {
                    // download
                    DownloadFileName =
                        $"{Report.Name}_{FileTool.CurrentTimeStamp()}{documentType.GetFileExtension()}";
                    var reportName = Report.GetLocalizedName(Language);

                    // document stream
                    MemoryStream documentStream = null;
                    switch (documentType)
                    {
                        case DocumentType.Excel:
                            documentStream = DataMerge.ExcelMerge(dataSet, documentMetadata);
                            break;
                        case DocumentType.Word:
                        case DocumentType.Pdf:
                            if (ReportTemplate != null)
                            {
                                documentStream = DataMerge.Merge(
                                    new MemoryStream(Convert.FromBase64String(ReportTemplate.Content)),
                                    dataSet, documentType, documentMetadata);
                            }
                            else
                            {
                                await UserNotification.ShowErrorMessageBoxAsync($"Report {reportName}",
                                    $"Report {reportName} without template in language {User.Language}");
                            }
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
                                await UserNotification.ShowErrorMessageBoxAsync($"Report {reportName}",
                                   $"Invalid XML for report {reportName}, please check the XSD schema");
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
                                await UserNotification.ShowErrorMessageBoxAsync($"Report {reportName}",
                                    $"Empty XML raw report {reportName}");
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
                Language = User.Language
            };
            if (reportSet.Parameters != null)
            {
                reportRequest.Parameters = reportSet.Parameters.ToDictionary(p => p.Name, p => p.Value);
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
                await UserNotification.ShowErrorMessageBoxAsync("Report start", exception);
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
                ReportTemplate = (await PayrollService.GetReportTemplatesAsync<Client.Model.ReportTemplate>(
                    new(Tenant.Id, Payroll.Id), new[] { Report.Name }, User.Language)).FirstOrDefault();
                if (ReportTemplate == null)
                {
                    await UserNotification.ShowErrorMessageBoxAsync("Report start", "Missing report template");
                }
            }
            catch (HttpRequestException exception)
            {
                if (exception.StatusCode != HttpStatusCode.NotFound)
                {
                    Log.Error(exception, exception.GetBaseMessage());
                    await UserNotification.ShowErrorMessageBoxAsync("Report start", exception);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception, exception.GetBaseMessage());
                await UserNotification.ShowErrorMessageBoxAsync("Report start", exception);
            }
        }

        private void ApplySystemParameters(ReportSet reportSet)
        {
            foreach (var parameter in reportSet.Parameters)
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
                    // tenant
                    case ReportParameterType.TenantId:
                        parameter.ValueAsInteger = Tenant.Id;
                        break;
                    // user
                    case ReportParameterType.UserId:
                        parameter.ValueAsInteger = User.Id;
                        break;
                    // regulation
                    case ReportParameterType.RegulationId:
                        parameter.ValueAsInteger = reportSet.RegulationId;
                        break;
                    // payroll
                    case ReportParameterType.PayrollId:
                        parameter.ValueAsInteger = Payroll.Id;
                        break;
                    // report
                    case ReportParameterType.ReportId:
                        parameter.ValueAsInteger = reportSet.Id;
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

            foreach (var parameter in report.Parameters)
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

            foreach (var parameter in report.Parameters)
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
        }
    }
}
