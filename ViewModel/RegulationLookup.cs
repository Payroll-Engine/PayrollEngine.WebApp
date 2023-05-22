﻿using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationLookup : Lookup, IRegulationItem, IKeyEquatable<RegulationLookup>
{
    public RegulationLookup()
    {
    }

    public RegulationLookup(RegulationLookup copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    protected RegulationLookup(Lookup copySource) :
        base(copySource)
    {
    }

    #region Regulation Object

    /// <inheritdoc />
    public int RegulationId { get; set; }

    /// <inheritdoc />
    public string RegulationName { get; set; }

    /// <inheritdoc />
    public RegulationInheritanceType InheritanceType { get; set; }

    /// <inheritdoc />
    public IRegulationItem BaseItem { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public RegulationItemType ItemType => RegulationItemType.Lookup;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => Name;

    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey => null;

    /// <inheritdoc />
    [JsonIgnore]
    public string AdditionalInfo => null;

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationLookup(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationLookup lookup)
        {
            lookup.Name = Name;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationLookup compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as RegulationLookup);

    public bool EqualKey(RegulationLookup compare) =>
        base.EqualKey(compare);
}