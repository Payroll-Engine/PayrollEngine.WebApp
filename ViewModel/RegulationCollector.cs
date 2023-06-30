using System;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationCollector : Collector, IRegulationItem, IKeyEquatable<RegulationCollector>
{
    public RegulationCollector()
    {
    }

    private RegulationCollector(RegulationCollector copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    protected RegulationCollector(Collector copySource) :
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
    public RegulationItemType ItemType => RegulationItemType.Collector;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => Name;
    
    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey => null;

    /// <inheritdoc />
    [JsonIgnore]
    public string Description => null;

    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) => 
        Enum.GetName(typeof(CollectType), CollectType);

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationCollector(this);
    
    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationCollector collector)
        {
            collector.Name = Name;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationCollector compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as RegulationCollector);

    public bool EqualKey(RegulationCollector compare) =>
        base.EqualKey(compare);
}