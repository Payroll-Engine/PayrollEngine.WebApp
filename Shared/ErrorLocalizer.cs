using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ErrorLocalizer : LocalizerBase
{
    public ErrorLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Error => PropertyValue();
    public string DeleteFailed => PropertyValue();
    public string FileDownloadError => PropertyValue();
    public string FileUploadError => PropertyValue();
    public string EmptyCollection => PropertyValue();
    public string MissingMandatoryValue => PropertyValue();
    public string JsonFormatError => PropertyValue();

    public string MissingEmployee(string identifier) =>
        FormatValue(PropertyValue(), nameof(identifier), identifier);
    public string RequiredField(string field) =>
        FormatValue(PropertyValue(), nameof(field), field);

    public string UnknownItem(string type, object item) =>
        FormatValue(PropertyValue(), nameof(type), type, nameof(item), item);
    public string UniqueConflict(string key) =>
        FormatValue(PropertyValue(), nameof(key), key);
    public string EmptyActionField(string field) =>
        FormatValue(PropertyValue(), nameof(field), field);
}
