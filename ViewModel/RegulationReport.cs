using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationReport : ReportSet, IRegulationItem, IKeyEquatable<RegulationReport>
{
    public RegulationReport()
    {
    }

    private RegulationReport(RegulationReport copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    protected RegulationReport(ReportSet copySource) :
        base(copySource)
    {
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
    public string InheritanceKey => Name;

    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey => null;

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

    public bool EqualKey(RegulationReport compare) =>
        base.EqualKey(compare);
}