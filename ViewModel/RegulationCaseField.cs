using System;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationCaseField : CaseField, IRegulationItem, IKeyEquatable<RegulationCaseField>
{
    public RegulationCaseField()
    {
    }

    public RegulationCaseField(RegulationCaseField copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    protected RegulationCaseField(CaseField copySource) :
        base(copySource)
    {
    }

    #region Regulation Object

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

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
    public RegulationItemType ItemType => RegulationItemType.CaseField;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => Name;

    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey =>
        Parent?.InheritanceKey;

    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) => 
        Enum.GetName(typeof(ValueType), ValueType);

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationCaseField(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationCaseField caseField)
        {
            caseField.Name = Name;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationCaseField compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as RegulationCaseField);

    public bool EqualKey(RegulationCaseField compare) =>
        base.EqualKey(compare);
}