using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseFieldLocalizer : LocalizerBase
{
    public CaseFieldLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CaseField => FromCaller();
    public string CaseFields => FromCaller();


    public string CancellationMode => FromCaller();
    public string TimeType => FromCaller();
    public string TimeUnit => FromCaller();
    public string StartDateType => FromCaller();
    public string DefaultStart => FromCaller();
    public string EndDateType => FromCaller();
    public string DefaultEnd => FromCaller();
    public string EndMandatory => FromCaller();
    public string ValueScope => FromCaller();
    public string ValueCreationMode => FromCaller();
    public string DefaultValue => FromCaller();
    public string ValueMandatory => FromCaller();
    public string LookupSettings => FromCaller();
    public string LookupSettingsRemoved => FromCaller();

    public string BuildActions => FromCaller();
    public string ValidateActions => FromCaller();

    public string TimeGroup => FromCaller();
    public string FieldDocuments => FromCaller();
}