using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class Localizer : LocalizerBase
{
    public Localizer(IStringLocalizerFactory factory) :
        base(factory, nameof(Localizer))
    {
        Shared = new(factory);
        Error = new(factory);
        Dialog = new(factory);
        Item = new(factory);
        Localization = new(factory);
        Login = new(factory);

        Tenant = new(factory);
        User = new(factory);
        Storage = new(factory);
        Division = new(factory);
        Employee = new(factory);
        Log = new(factory);
        Task = new(factory);
        Webhook = new(factory);
        WebhookMessage = new(factory);

        Calendar = new(factory);
        Document = new(factory);
        Attribute = new(factory);
        Cluster = new(factory);
        ClusterSet = new(factory);

        Case = new(factory);
        CaseField = new(factory);
        CaseValue = new(factory);
        CaseRelation = new(factory);
        CaseChange = new(factory);
        CaseSlot = new(factory);
        Action = new(factory);
        Script = new(factory);
        Lookup = new(factory);
        LookupValue = new(factory);
        Collector = new(factory);
        WageType = new(factory);

        Report = new(factory);
        ReportLog = new(factory);
        ReportQuery = new(factory);
        ReportParameter = new(factory);
        ReportTemplate = new(factory);

        Payroll = new(factory);
        PayrollLayer = new(factory);
        Regulation = new(factory);
        RegulationShare = new(factory);

        Payrun = new(factory);
        PayrunJob = new(factory);
        PayrunParameter = new(factory);
        PayrunResult = new(factory);

        Forecast = new(factory);

        DataGrid = new(factory);
        App = new(factory);
    }

    public SharedLocalizer Shared { get; }
    public ErrorLocalizer Error { get; }
    public DialogLocalizer Dialog { get; }
    public ItemLocalizer Item { get; }
    public LocalizationLocalizer Localization { get; }
    public LoginLocalizer Login { get; }

    public TenantLocalizer Tenant { get; }
    public UserLocalizer User { get; }
    public StorageLocalizer Storage { get; }
    public DivisionLocalizer Division { get; }
    public EmployeeLocalizer Employee { get; }
    public LogLocalizer Log { get; }
    public TaskLocalizer Task { get; }
    public WebhookLocalizer Webhook { get; }
    public WebhookMessageLocalizer WebhookMessage { get; }

    public CalendarLocalizer Calendar { get; }
    public DocumentLocalizer Document { get; }
    public AttributeLocalizer Attribute { get; }
    public ClusterLocalizer Cluster { get; }
    public ClusterSetLocalizer ClusterSet { get; }

    public CaseLocalizer Case { get; }
    public CaseFieldLocalizer CaseField { get; }
    public CaseValueLocalizer CaseValue { get; }
    public CaseRelationLocalizer CaseRelation { get; }
    public CaseChangeLocalizer CaseChange { get; }
    public CaseSlotLocalizer CaseSlot { get; }
    public ActionLocalizer Action { get; }
    public ScriptLocalizer Script { get; }
    public LookupLocalizer Lookup { get; }
    public LookupValueLocalizer LookupValue { get; }
    public CollectorLocalizer Collector { get; }
    public WageTypeLocalizer WageType { get; }

    public ReportLocalizer Report { get; }
    public ReportLogLocalizer ReportLog { get; }
    public ReportQueryLocalizer ReportQuery { get; }
    public ReportParameterLocalizer ReportParameter { get; }
    public ReportTemplateLocalizer ReportTemplate { get; }

    public PayrollLocalizer Payroll { get; }
    public PayrollLayerLocalizer PayrollLayer { get; }
    public RegulationLocalizer Regulation { get; }
    public RegulationShareLocalizer RegulationShare { get; }

    public PayrunLocalizer Payrun { get; }
    public PayrunJobLocalizer PayrunJob { get; }
    public PayrunParameterLocalizer PayrunParameter { get; }
    public PayrunResultLocalizer PayrunResult { get; }

    public ForecastLocalizer Forecast { get; }

    public DataGridLocalizer DataGrid { get; }
    public AppLocalizer App { get; }
}