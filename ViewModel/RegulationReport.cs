using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model regulation report
/// </summary>
public class RegulationReport : ReportSet, IRegulationItem, IKeyEquatable<RegulationReport>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationReport()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    private RegulationReport(RegulationReport copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    #region Regulation Object

    /// <inheritdoc />
    public string RegulationName { get; set; }

    /// <inheritdoc />
    public RegulationInheritanceType InheritanceType { get; set; }

    /// <inheritdoc />
    public IRegulationItem BaseItem { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public RegulationItemType ItemType => RegulationItemType.Report;

    /// <inheritdoc />
    [JsonIgnore]
    public int ExpressionCount =>
        (string.IsNullOrWhiteSpace(BuildExpression) ? 0 : 1) +
        (string.IsNullOrWhiteSpace(StartExpression) ? 0 : 1) +
        (string.IsNullOrWhiteSpace(EndExpression) ? 0 : 1);

    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) => Category;

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationReport(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationReport report)
        {
            report.Name = Name;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationReport compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <inheritdoc />
    public bool EqualKey(RegulationReport compare) =>
        base.EqualKey(compare);
}