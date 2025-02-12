using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class Localizer(IStringLocalizerFactory factory) : LocalizerBase(factory, nameof(Localizer))
{
    public SharedLocalizer Shared { get; } = new(factory);
    public ErrorLocalizer Error { get; } = new(factory);
    public DialogLocalizer Dialog { get; } = new(factory);
    public ItemLocalizer Item { get; } = new(factory);
    public LocalizationLocalizer Localization { get; } = new(factory);
    public LoginLocalizer Login { get; } = new(factory);

    public TenantLocalizer Tenant { get; } = new(factory);
    public UserLocalizer User { get; } = new(factory);
    public StorageLocalizer Storage { get; } = new(factory);
    public DivisionLocalizer Division { get; } = new(factory);
    public EmployeeLocalizer Employee { get; } = new(factory);
    public LogLocalizer Log { get; } = new(factory);
    public TaskLocalizer Task { get; } = new(factory);
    public WebhookLocalizer Webhook { get; } = new(factory);
    public WebhookMessageLocalizer WebhookMessage { get; } = new(factory);

    public CalendarLocalizer Calendar { get; } = new(factory);
    public DocumentLocalizer Document { get; } = new(factory);

    public AttributeLocalizer Attribute { get; } = new(factory);

    public ClusterSetLocalizer ClusterSet { get; } = new(factory);

    public CaseLocalizer Case { get; } = new(factory);
    public CaseFieldLocalizer CaseField { get; } = new(factory);
    public CaseValueLocalizer CaseValue { get; } = new(factory);
    public CaseRelationLocalizer CaseRelation { get; } = new(factory);
    public CaseChangeLocalizer CaseChange { get; } = new(factory);
    public CaseSlotLocalizer CaseSlot { get; } = new(factory);
    public ActionLocalizer Action { get; } = new(factory);
    public ScriptLocalizer Script { get; } = new(factory);
    public LookupLocalizer Lookup { get; } = new(factory);
    public LookupValueLocalizer LookupValue { get; } = new(factory);
    public CollectorLocalizer Collector { get; } = new(factory);
    public WageTypeLocalizer WageType { get; } = new(factory);

    public ReportLocalizer Report { get; } = new(factory);
    public ReportLogLocalizer ReportLog { get; } = new(factory);
    public ReportQueryLocalizer ReportQuery { get; } = new(factory);
    public ReportParameterLocalizer ReportParameter { get; } = new(factory);
    public ReportTemplateLocalizer ReportTemplate { get; } = new(factory);

    public PayrollLocalizer Payroll { get; } = new(factory);
    public PayrollLayerLocalizer PayrollLayer { get; } = new(factory);
    public RegulationLocalizer Regulation { get; } = new(factory);
    public RegulationShareLocalizer RegulationShare { get; } = new(factory);

    public PayrunLocalizer Payrun { get; } = new(factory);
    public PayrunJobLocalizer PayrunJob { get; } = new(factory);
    public PayrunParameterLocalizer PayrunParameter { get; } = new(factory);
    public PayrunResultLocalizer PayrunResult { get; } = new(factory);

    public ForecastLocalizer Forecast { get; } = new(factory);

    // blazor controls
    public NavGroupLocalizer NavGroup { get; } = new(factory);
    public DataGridLocalizer DataGrid { get; } = new(factory);
    public DataGridPagerLocalizer DataGridPager { get; } = new(factory);
    public DatePickerLocalizer DataPicker { get; } = new(factory);
    public InputLocalizer Input { get; } = new(factory);
    public SnackBarLocalizer SnackBar { get; } = new(factory);

    public AppLocalizer App { get; } = new(factory);
}