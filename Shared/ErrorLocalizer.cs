using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ErrorLocalizer : LocalizerBase
{
    public ErrorLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Error => FromCaller();
    public string DeleteFailed => FromCaller();
    public string FileDownloadError => FromCaller();
    public string FileUploadError => FromCaller();
    public string EmptyCollection => FromCaller();
    public string MissingMandatoryValue => FromCaller();
    public string JsonFormatError => FromCaller();

    public string MissingEmployee(string identifier) =>
        string.Format(FromCaller(), identifier);
    public string RequiredField(string fieldName) =>
        string.Format(FromCaller(), fieldName);
    public string UnknownItem(string type, object item) =>
        string.Format(FromCaller(), type, item);
    public string UniqueConflict(string name) =>
        string.Format(FromCaller(), name);
    public string EmptyActionField(string name) =>
        string.Format(FromCaller(), name);
}