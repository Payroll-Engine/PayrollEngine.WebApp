using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class Localizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture, nameof(Localizer))
{
    public SharedLocalizer Shared { get; } = new(factory, culture);
    public ErrorLocalizer Error { get; } = new(factory, culture);
    public DialogLocalizer Dialog { get; } = new(factory, culture);
    public ItemLocalizer Item { get; } = new(factory, culture);
    public LocalizationLocalizer Localization { get; } = new(factory, culture);
    public LoginLocalizer Login { get; } = new(factory, culture);

    public TenantLocalizer Tenant { get; } = new(factory, culture);
    public UserLocalizer User { get; } = new(factory, culture);
    public StorageLocalizer Storage { get; } = new(factory, culture);
    public DivisionLocalizer Division { get; } = new(factory, culture);
    public EmployeeLocalizer Employee { get; } = new(factory, culture);
    public LogLocalizer Log { get; } = new(factory, culture);
    public TaskLocalizer Task { get; } = new(factory, culture);
    public WebhookLocalizer Webhook { get; } = new(factory, culture);
    public WebhookMessageLocalizer WebhookMessage { get; } = new(factory, culture);

    public CalendarLocalizer Calendar { get; } = new(factory, culture);
    public DocumentLocalizer Document { get; } = new(factory, culture);

    public AttributeLocalizer Attribute { get; } = new(factory, culture);

    public ClusterSetLocalizer ClusterSet { get; } = new(factory, culture);

    public CaseLocalizer Case { get; } = new(factory, culture);
    public CaseFieldLocalizer CaseField { get; } = new(factory, culture);
    public CaseValueLocalizer CaseValue { get; } = new(factory, culture);
    public CaseRelationLocalizer CaseRelation { get; } = new(factory, culture);
    public CaseChangeLocalizer CaseChange { get; } = new(factory, culture);
    public CaseSlotLocalizer CaseSlot { get; } = new(factory, culture);
    public ActionLocalizer Action { get; } = new(factory, culture);
    public ScriptLocalizer Script { get; } = new(factory, culture);
    public LookupLocalizer Lookup { get; } = new(factory, culture);
    public LookupValueLocalizer LookupValue { get; } = new(factory, culture);
    public CollectorLocalizer Collector { get; } = new(factory, culture);
    public WageTypeLocalizer WageType { get; } = new(factory, culture);

    public ReportLocalizer Report { get; } = new(factory, culture);
    public ReportLogLocalizer ReportLog { get; } = new(factory, culture);
    public ReportQueryLocalizer ReportQuery { get; } = new(factory, culture);
    public ReportParameterLocalizer ReportParameter { get; } = new(factory, culture);
    public ReportTemplateLocalizer ReportTemplate { get; } = new(factory, culture);

    public PayrollLocalizer Payroll { get; } = new(factory, culture);
    public PayrollLayerLocalizer PayrollLayer { get; } = new(factory, culture);
    public RegulationLocalizer Regulation { get; } = new(factory, culture);
    public RegulationShareLocalizer RegulationShare { get; } = new(factory, culture);

    public PayrunLocalizer Payrun { get; } = new(factory, culture);
    public PayrunJobLocalizer PayrunJob { get; } = new(factory, culture);
    public PayrunParameterLocalizer PayrunParameter { get; } = new(factory, culture);
    public PayrunResultLocalizer PayrunResult { get; } = new(factory, culture);

    public ForecastLocalizer Forecast { get; } = new(factory, culture);

    // blazor controls
    public NavGroupLocalizer NavGroup { get; } = new(factory, culture);
    public DataGridLocalizer DataGrid { get; } = new(factory, culture);
    public DataGridPagerLocalizer DataGridPager { get; } = new(factory, culture);
    public DatePickerLocalizer DataPicker { get; } = new(factory, culture);
    public InputLocalizer Input { get; } = new(factory, culture);
    public SnackBarLocalizer SnackBar { get; } = new(factory, culture);

    public AppLocalizer App { get; } = new(factory, culture);
}