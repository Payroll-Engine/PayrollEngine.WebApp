using System;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model regulation case
/// </summary>
public class RegulationCase : Case, IRegulationItem, IKeyEquatable<RegulationCase>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationCase()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    private RegulationCase(RegulationCase copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    protected RegulationCase(Client.Model.Case copySource) :
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
    public RegulationItemType ItemType => RegulationItemType.Case;

    /// <inheritdoc />
    [JsonIgnore]
    public int ActionCount =>
        (AvailableActions?.Count ?? 0) +
        (BuildActions?.Count ?? 0) +
        (ValidateActions?.Count ?? 0);

    /// <inheritdoc />
    [JsonIgnore]
    public int ExpressionCount =>
        (string.IsNullOrWhiteSpace(AvailableExpression) ? 0 : 1) +
        (string.IsNullOrWhiteSpace(BuildExpression) ? 0 : 1) +
        (string.IsNullOrWhiteSpace(ValidateExpression) ? 0 : 1);

    /// <inheritdoc />
    public int GetGroupCount(string groupName)
    {
        if (string.Equals(groupName, nameof(Slots)) && Slots != null)
        {
            return Slots.Count;
        }
        return 0;
    }

    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) =>
        Enum.GetName(typeof(CaseType), CaseType);

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationCase(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationCase targetCase)
        {
            targetCase.Name = Name;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationCase compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <remarks>Do not call the base class method</remarks>
    /// <returns>True for objects with the same data</returns>
    public override bool Equals(IViewModel compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <inheritdoc />
    public bool EqualKey(RegulationCase compare) =>
        base.EqualKey(compare);
}