using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class Report : Client.Model.Report, IViewModel,
    IViewAttributeObject, IKeyEquatable<Report>
{
    public Report()
    {
    }

    public Report(Report copySource) :
        base(copySource)
    {
    }

    public Report(Client.Model.Report copySource) :
        base(copySource)
    {
    }

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? default;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? default;

    #endregion

    public string GetLocalizedName(Language language) =>
        language.GetLocalization(NameLocalizations, Name);

    public string GetLocalizedDescription(Language language) =>
        language.GetLocalization(DescriptionLocalizations, Description);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Report compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as Report);

    public bool EqualKey(Report compare) =>
        base.EqualKey(compare);
}