using System.Linq;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model report set
/// </summary>
public class ReportSet : Client.Model.ReportSet, IViewModel,
    IViewAttributeObject, IKeyEquatable<ReportSet>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public ReportSet()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public ReportSet(ReportSet copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public ReportSet(Report copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
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
            viewParameters = [];
            if (Parameters == null)
            {
                return viewParameters;
            }
            // select visible parameters
            foreach (var parameter in Parameters.Where(x => !x.Hidden))
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
            viewTemplates = [];
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
        Attributes?.GetDecimalAttributeValue(name) ?? 0;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? false;

    #endregion

    /// <summary>
    /// Get the localized name
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedName(string culture) =>
        culture.GetLocalization(NameLocalizations, Name);

    /// <summary>
    /// Get localized description
    /// </summary>
    /// <param name="culture">Culture</param>
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

    /// <inheritdoc />
    public bool EqualKey(ReportSet compare) =>
        base.EqualKey(compare);
}