using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Net.Http;
using System.Globalization;
using System.Collections.Generic;
using DataSet = System.Data.DataSet;
using Task = System.Threading.Tasks.Task;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using PayrollEngine.IO;
using PayrollEngine.Data;
using PayrollEngine.Client;
using PayrollEngine.Document;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.Client.Service;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Report;

public partial class ReportBuildDialog
{
    private MudForm form;

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; }
    [Parameter] public Tenant Tenant { get; set; }
    [Parameter] public User User { get; set; }
    [Parameter] public Client.Model.Payroll Payroll { get; set; }
    [Parameter] public ReportSet Report { get; set; }
    [Parameter] public CultureInfo Culture { get; set; }
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
    private ILocalizerService LocalizerService { get; set; }

    private Localizer Localizer => LocalizerService.Localizer;
    private ReportSet EditReport { get; set; }
    private Client.Model.ReportTemplate ReportTemplate { get; set; }

    private bool Valid { get; set; }
    private bool Executing { get; set; }
    private bool Completed { get; set; }
    private bool Failed { get; set; }
    private bool Started => Executing || Completed || Failed;

    private string ErrorMessage { get; set; }
    private string DownloadFileName { get; set; }

    private string ReportName => EditReport.GetLocalizedName(Culture.Name);

    private string ReportDescription => EditReport.GetLocalizedDescription(Culture.Name);
    private bool HasDescription =>
        !string.IsNullOrWhiteSpace(ReportDescription);

    private bool HasVisibleParameters() =>
        EditReport.ViewParameters == null ||
        EditReport.ViewParameters.Count(x => x.Attributes?.GetHidden(Culture) ?? false) != EditReport.ViewParameters.Count;

    private bool SupportedDocumentType(DocumentType documentType) =>
        documentType == DocumentType.Excel || DataMerge.IsMergeable(documentType);

    /// <summary>
    /// Case info from build and validate
    /// </summary>
    private Dictionary<string, object> ReportInfo { get; set; }

    #region Actions

    protected void Close()
    {
        if (Completed)
        {
            MudDialog.Close(DialogResult.Ok(true));
            return;
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
                Localizer.Report.TemplateNotAvailable(ReportName, Culture.Name));
            return;
        }

        try
        {
            Executing = true;
            StateHasChanged();

            // generate parameter dictionary
            var parameters = EditReport.ViewParameters.ToDictionary(p => p.Name, p => p.Value);

            var response = await ReportService.ExecuteReportAsync(
                new(Tenant.Id, EditReport.RegulationId), EditReport.Id,
                new()
                {
                    Culture = Culture.Name,
                    Parameters = parameters,
                    UserId = User.Id
                });

            // report metadata
            var now = DateTime.Now; // use local time (no UTC)
            var title = EditReport.GetLocalizedName(Culture.Name);
            var documentMetadata = new DocumentMetadata
            {
                Author = User.Identifier,
                Category = EditReport.Category,
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
                    $"{EditReport.Name}_{FileTool.CurrentTimeStamp()}{documentType.GetFileExtension()}";
                var reportName = EditReport.GetLocalizedName(Culture.Name);

                var mergeParameters = new Dictionary<string, object>(parameters.Select(x => new KeyValuePair<string, object>(x.Key, x.Value)));

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
                            new MemoryStream(Encoding.ASCII.GetBytes(ReportTemplate.Content)),
                            dataSet, documentType, documentMetadata, mergeParameters);
                        break;
                    case DocumentType.Xml:
                    case DocumentType.XmlRaw:
                        // xml
                        var xml = XmlTool.IsContentTypeXsl(ReportTemplate.ContentType) ?
                            // xsl report
                            XmlTool.TransformXmlFromXsl(dataSet, ReportTemplate.Content) :
                            // dataset report
                            XmlTool.TransformXml(dataSet);
                        if (string.IsNullOrWhiteSpace(xml))
                        {
                            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report,
                                Localizer.Report.EmptyXmlRaw(reportName));
                            break;
                        }

                        // xsd validation
                        if (!string.IsNullOrWhiteSpace(ReportTemplate.Schema) &&
                            !XmlTool.ValidateXmlString(xml, ReportTemplate.Schema))
                        {
                            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report,
                                Localizer.Report.XmlValidationError(reportName));
                            break;
                        }

                        // xml stream
                        documentStream = XmlTool.XmlToMemoryStream(xml);
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

    #endregion

    #region Report Load

    /// <summary>
    /// Build or rebuild the report including the report parameters
    /// </summary>
    private async Task UpdateReportAsync()
    {
        // prepare request
        var reportRequest = new Client.Model.ReportRequest
        {
            UserId = User.Id,
            Culture = User.Culture
        };
        if (EditReport.ViewParameters != null)
        {
            reportRequest.Parameters = EditReport.ViewParameters.ToDictionary(p => p.Name, p => p.Value);
        }

        // get report and merge
        try
        {
            // retrieve report with updated parameters
            var buildReport = await ReportSetService.GetAsync<ReportSet>(
                new(Tenant.Id, EditReport.RegulationId), EditReport.Id, reportRequest);
            // convert to view model
            ReportMerger.Merge(buildReport, EditReport);

            // register new report handler
            AddReportHandler();
            ApplySystemParameters();
            UpdateInfo();
            await InvokeAsync(UpdateValidation);
        }
        catch (Exception exception)
        {
            Log.Error(exception, exception.GetBaseMessage());
            await UserNotification.ShowErrorMessageBoxAsync(Localizer, Localizer.Report.Report, exception);
        }
    }

    /// <summary>
    /// Update validation
    /// </summary>
    private void UpdateValidation()
    {
        // valid state
        var valid = ValidBuild;
        if (Valid != valid)
        {
            Valid = valid;
        }
        // update the state in case of info changes
        StateHasChanged();
    }

    private bool ValidBuild =>
        EditReport.Attributes.GetValidity(Culture) ?? true;

    private void UpdateInfo() =>
        ReportInfo = EditReport.Attributes.GetEditInfo();

    /// <summary>
    /// Set up the report template
    /// </summary>
    private async Task SetupReportTemplateAsync()
    {
        try
        {
            // template by culture
            ReportTemplate = (await PayrollService.GetReportTemplatesAsync<Client.Model.ReportTemplate>(
                new(Tenant.Id, Payroll.Id),
                reportNames: [EditReport.Name],
                culture: Culture.Name)).FirstOrDefault();

            // fallback 1: template to base culture
            if (ReportTemplate == null)
            {
                var index = Culture.Name.IndexOf('-');
                if (index >= 0)
                {
                    var baseCulture = Culture.Name.Substring(0, index);
                    ReportTemplate = (await PayrollService.GetReportTemplatesAsync<Client.Model.ReportTemplate>(
                        new(Tenant.Id, Payroll.Id),
                        reportNames: [EditReport.Name],
                        culture: baseCulture)).FirstOrDefault();
                }
            }

            // fallback 2: first available template
            ReportTemplate ??= (await PayrollService.GetReportTemplatesAsync<Client.Model.ReportTemplate>(
                new(Tenant.Id, Payroll.Id),
                reportNames: [EditReport.Name])).FirstOrDefault();
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

    private void AddReportHandler()
    {
        foreach (var parameter in EditReport.ViewParameters)
        {
            //// tenant culture
            parameter.TenantCulture = Culture;
            // value formatter
            parameter.ValueFormatter = ValueFormatter;
            // change notification
            if (!parameter.HasParameterChangedListener)
            {
                parameter.ParameterChanged += ReportParameterChanged;
            }
        }
    }

    private void ApplySystemParameters()
    {
        foreach (var parameter in EditReport.ViewParameters)
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

    private void ReportParameterChanged(ReportParameter parameter) =>
        Task.Run(UpdateReportAsync);

    #endregion

    #region Lfecycle

    protected override async Task OnInitializedAsync()
    {
        // working copy, don't change report parameter values
        EditReport = new ReportSet(Report);

        await UpdateReportAsync();
        await SetupReportTemplateAsync();
        await base.OnInitializedAsync();
    }

    private bool initValidation;
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // validation
        if (!initValidation && form != null)
        {
            UpdateValidation();
            initValidation = true;
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    #endregion
}