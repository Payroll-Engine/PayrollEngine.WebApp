using System.Globalization;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model regulation wage type
/// </summary>
public class RegulationWageType : WageType, IRegulationItem, IKeyEquatable<RegulationWageType>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationWageType()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    private RegulationWageType(RegulationWageType copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    protected RegulationWageType(WageType copySource) :
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
    public RegulationItemType ItemType => RegulationItemType.WageType;

    /// <inheritdoc />
    [JsonIgnore]
    public int ActionCount =>
        (ValueActions?.Count ?? 0) +
        (ResultActions?.Count ?? 0);

    /// <inheritdoc />
    [JsonIgnore]
    public int ExpressionCount =>
        (string.IsNullOrWhiteSpace(ValueExpression) ? 0 : 1) +
        (string.IsNullOrWhiteSpace(ResultExpression) ? 0 : 1);

    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) => Name;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => 
        WageTypeNumber.ToString("0.####", CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationWageType(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationWageType wageType)
        {
            wageType.WageTypeNumber = WageTypeNumber;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationWageType compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as RegulationWageType);

    /// <inheritdoc />
    public bool EqualKey(RegulationWageType compare) =>
        base.EqualKey(compare);
}