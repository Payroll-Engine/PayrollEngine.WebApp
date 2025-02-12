using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseFieldLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string CaseField => PropertyValue();
    public string CaseFields => PropertyValue();

    public string CultureHelp => PropertyValue();
    public string CancellationMode => PropertyValue();
    public string TimeType => PropertyValue();
    public string TimeUnit => PropertyValue();
    public string StartDateType => PropertyValue();
    public string DefaultStart => PropertyValue();
    public string EndDateType => PropertyValue();
    public string DefaultEnd => PropertyValue();
    public string EndMandatory => PropertyValue();
    public string ValueScope => PropertyValue();
    public string ValueCreationMode => PropertyValue();
    public string DefaultValue => PropertyValue();
    public string ValueMandatory => PropertyValue();
    public string LookupSettings => PropertyValue();
    public string LookupSettingsRemoved => PropertyValue();
    public string BuildActions => PropertyValue();
    public string ValidateActions => PropertyValue();
    public string TimeGroup => PropertyValue();
    public string FieldDocuments => PropertyValue();
    public string MissingValue => PropertyValue();
    public string MissingStart => PropertyValue();
    public string MissingEnd => PropertyValue();
    public string MissingAttachment => PropertyValue();
    public string CaseFieldDocuments => PropertyValue();
    public string EmptyChangeHistory => PropertyValue();
    public string ChangeHistory => PropertyValue();
}