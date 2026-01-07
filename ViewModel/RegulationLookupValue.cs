using System.Globalization;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model regulation lookup value
/// </summary>
public class RegulationLookupValue : LookupValue, IRegulationItem, IKeyEquatable<RegulationLookupValue>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationLookupValue()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    private RegulationLookupValue(RegulationLookupValue copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    protected RegulationLookupValue(LookupValue copySource) :
        base(copySource)
    {
    }

    #region Regulation Object

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

    /// <inheritdoc />
    public string RegulationName { get; set; }

    /// <inheritdoc />
    public RegulationInheritanceType InheritanceType { get; set; }

    /// <inheritdoc />
    public IRegulationItem BaseItem { get; set; }

    /// <inheritdoc />
    public Dictionary<string, object> Attributes { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public RegulationItemType ItemType => RegulationItemType.LookupValue;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => Key;

    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey => Parent?.InheritanceKey;

    /// <inheritdoc />
    [JsonIgnore]
    public string Name => Key;
    
    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) =>
       RangeValue?.ToString(SystemSpecification.DecimalFormat, CultureInfo.InvariantCulture);

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationLookupValue(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationLookupValue lookupValue)
        {
            lookupValue.Key = Key;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationLookupValue compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as RegulationLookupValue);

    /// <inheritdoc />
    public bool EqualKey(RegulationLookupValue compare) =>
        base.EqualKey(compare);
}