using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class SharedLocalizer : LocalizerBase
{
    public SharedLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    // object
    public string ObjectId => FromCaller();
    public string ObjectStatus => FromCaller();
    public string ObjectCreated => FromCaller();
    public string ObjectUpdated => FromCaller();
    public string Immutable => FromCaller();

    public string All => FromCaller();
    public string SelectAll => FromCaller();
    public string None => FromCaller();
    public string NotAvailable => FromCaller();
    public string Localizations => FromCaller();
    public string Owner => FromCaller();
    public string Order => FromCaller();
    public string Tags => FromCaller();
    public string CommaSeparatedList => FromCaller();
    public string Mandatory => FromCaller();
    public string OverrideType => FromCaller();
    public string Clusters => FromCaller();

    public string Key => FromCaller();
    public string Field => FromCaller();
    public string Start => FromCaller();
    public string End => FromCaller();
    public string Value => FromCaller();
    public string NumericValue => FromCaller();
    public string Text => FromCaller();
    public string Type => FromCaller();
    public string ValueType => FromCaller();
    public string Search => FromCaller();

    // common
    public string Identifier => FromCaller();
    public string Name => FromCaller();
    public string Description => FromCaller();
    public string Inheritance => FromCaller();
    public string Culture => FromCaller();
    public string Calendar => FromCaller();
    public string Language => FromCaller();

    public string FeaturesAdmin => FromCaller();
    public string FeaturesSystem => FromCaller();
    public string CommonFields => FromCaller();

    // data
    public string FilterReset => FromCaller();
    public string ExcelDownload => FromCaller();
    public string DownloadCompleted => FromCaller();

    public string ImmediateChanges => FromCaller();
    public string JsonReformat => FromCaller();

    public string RequiredField(string name) =>
        string.Format(FromCaller(), name);
}