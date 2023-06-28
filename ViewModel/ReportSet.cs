using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class ReportSet : Client.Model.ReportSet, IViewModel,
    IViewAttributeObject, IKeyEquatable<ReportSet>
{
    public ReportSet()
    {
    }

    public ReportSet(ReportSet copySource) :
        base(copySource)
    {
        Parameters = new();
        if (copySource.Parameters != null)
        {
            Parameters.AddRangeAsync(copySource.Parameters).Wait();
        }
    }

    public ReportSet(Report copySource) :
        base(copySource)
    {
        Parameters = new();
    }

    public ReportSet(Client.Model.ReportSet copySource) :
        base(copySource)
    {
        Parameters = new();
        if (copySource.Parameters != null)
        {
            copySource.Parameters.ForEach(p =>
                Parameters.AddAsync(new(p)).Wait());
        }
    }

    /// <summary>The report parameters</summary>
    public new ObservedHashSet<ReportParameter> Parameters { get; set; }

    /// <summary>The report templates</summary>
    public new ObservedHashSet<ReportTemplate> Templates { get; set; }

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

    public string GetLocalizedName(string culture) =>
        culture.GetLocalization(NameLocalizations, Name);

    public string GetLocalizedDescription(string culture) =>
        culture.GetLocalization(DescriptionLocalizations, Description);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(ReportSet compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as ReportSet);

    public bool EqualKey(ReportSet compare) =>
        base.EqualKey(compare);
}