using System.Text;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model regulation case relation
/// </summary>
public class RegulationCaseRelation : CaseRelation, IRegulationItem, IKeyEquatable<RegulationCaseRelation>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationCaseRelation()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    private RegulationCaseRelation(RegulationCaseRelation copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    protected RegulationCaseRelation(CaseRelation copySource) :
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
    public RegulationItemType ItemType => RegulationItemType.CaseRelation;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => GetInheritanceKey();

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => ToString();
    
    /// <inheritdoc />
    [JsonIgnore]
    public int ActionCount =>
        (BuildActions?.Count ?? 0) +
        (ValidateActions?.Count ?? 0);

    /// <inheritdoc />
    [JsonIgnore]
    public int ExpressionCount =>
        (string.IsNullOrWhiteSpace(BuildExpression) ? 0 : 1) +
        (string.IsNullOrWhiteSpace(ValidateExpression) ? 0 : 1);

    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) => null;

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationCaseRelation(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationCaseRelation caseRelation)
        {
            caseRelation.SourceCaseName = SourceCaseName;
            caseRelation.SourceCaseSlot = SourceCaseSlot;
            caseRelation.TargetCaseName = TargetCaseName;
            caseRelation.TargetCaseSlot = TargetCaseSlot;
        }
    }

    #endregion

    private string GetInheritanceKey()
    {
        if (string.IsNullOrWhiteSpace(SourceCaseName) && string.IsNullOrWhiteSpace(TargetCaseName) &&
            string.IsNullOrWhiteSpace(SourceCaseSlot) && string.IsNullOrWhiteSpace(TargetCaseSlot))
        {
            return string.Empty;
        }

        var buffer = new StringBuilder();

        // source case
        buffer.Append(SourceCaseName);
        if (!string.IsNullOrWhiteSpace(SourceCaseSlot))
        {
            buffer.Append($".{SourceCaseSlot}");
        }

        buffer.Append(" > ");

        // target case
        buffer.Append(TargetCaseName);
        if (!string.IsNullOrWhiteSpace(TargetCaseSlot))
        {
            buffer.Append($".{TargetCaseSlot}");
        }

        return buffer.ToString();
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationCaseRelation compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as RegulationCaseRelation);

    /// <inheritdoc />
    public bool EqualKey(RegulationCaseRelation compare) =>
        base.EqualKey(compare);
}