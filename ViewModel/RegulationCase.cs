using System;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationCase : Case, IRegulationItem, IKeyEquatable<RegulationCase>
{
    public RegulationCase()
    {
    }

    public RegulationCase(RegulationCase copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    protected RegulationCase(Client.Model.Case copySource) :
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
    public RegulationItemType ItemType => RegulationItemType.Case;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => Name;

    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey => null;

    [JsonIgnore]
    public string AdditionalInfo => Enum.GetName(typeof(CaseType), CaseType);

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

    public bool EqualKey(RegulationCase compare) =>
        base.EqualKey(compare);
}