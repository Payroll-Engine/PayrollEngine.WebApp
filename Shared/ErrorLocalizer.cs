using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ErrorLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Error => PropertyValue();
    public string FileDownloadError => PropertyValue();
    public string FileUploadError => PropertyValue();
    public string EmptyCollection => PropertyValue();
    public string MissingMandatoryValue => PropertyValue();
    public string JsonFormatError => PropertyValue();

    public string MissingEmployee(string identifier) =>
        FormatValue(PropertyValue(), nameof(identifier), identifier);
    public string RequiredField(string field) =>
        FormatValue(PropertyValue(), nameof(field), field);

    public string ItemRead(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string ItemCreate(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string ItemDerive(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string ItemUpdate(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);
    public string ItemDelete(string type) =>
        FormatValue(PropertyValue(), nameof(type), type);

    public string UnknownItem(string type, object item) =>
        FormatValue(PropertyValue(), nameof(type), type, nameof(item), item);
    public string UniqueConflict(string key) =>
        FormatValue(PropertyValue(), nameof(key), key);
    public string EmptyActionField(string field) =>
        FormatValue(PropertyValue(), nameof(field), field);
}
