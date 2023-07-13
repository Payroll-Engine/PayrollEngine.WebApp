using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class ReportSet : Client.Model.ReportSet, IViewModel,
    IViewAttributeObject, IKeyEquatable<ReportSet>
{
    // ReSharper disable once MemberCanBeProtected.Global
    public ReportSet()
    {
    }

    public ReportSet(ReportSet copySource) :
        base(copySource)
    {
    }

    public ReportSet(Report copySource) :
        base(copySource)
    {
    }

    public ReportSet(Client.Model.ReportSet copySource) :
        base(copySource)
    {
    }

    /// <summary>The report parameters</summary>
    private ObservedHashSet<ReportParameter> viewParameters;

    public ObservedHashSet<ReportParameter> ViewParameters
    {
        get
        {
            if (viewParameters != null)
            {
                return viewParameters;
            }
            viewParameters = new();
            if (Parameters == null)
            {
                return viewParameters;
            }
            foreach (var parameter in Parameters)
            {
                viewParameters.Add(new(parameter));
            }
            return viewParameters;
        }
    }

    /// <summary>The report templates</summary>
    private ObservedHashSet<ReportTemplate> viewTemplates;
    public ObservedHashSet<ReportTemplate> ViewTemplates
    {
        get
        {
            if (viewTemplates != null)
            {
                return viewTemplates;
            }
            viewTemplates = new();
            if (Templates == null)
            {
                return viewTemplates;
            }
            foreach (var template in Templates)
            {
                viewTemplates.Add(new(template));
            }
            return viewTemplates;
        }
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