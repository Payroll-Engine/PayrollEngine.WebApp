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
        Attributes?.GetDecimalAttributeValue(name) ?? 0;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? false;

    #endregion

    public string GetLocalizedName(string culture) =>
        culture.GetLocalization(NameLocalizations, Name);

    public string GetLocalizedDescription(string culture) =>
        culture.GetLocalization(DescriptionLocalizations, Description);

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