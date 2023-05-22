using PayrollEngine.Client.Model;
using System;
using System.Text.Json.Serialization;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationReportParameter : ReportParameter, IRegulationItem, IKeyEquatable<RegulationReportParameter>
{
    public RegulationReportParameter()
    {
    }

    public RegulationReportParameter(RegulationReportParameter copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    protected RegulationReportParameter(ReportParameter copySource) :
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
    public RegulationItemType ItemType => RegulationItemType.ReportParameter;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => Name;
    
    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey => Parent?.InheritanceKey;

    /// <inheritdoc />
    [JsonIgnore]
    public string AdditionalInfo => Enum.GetName(typeof(ValueType), ValueType);

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationReportParameter(this);
    
    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationReportParameter reportParameter)
        {
            reportParameter.Name = Name;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationReportParameter compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    public bool EqualKey(RegulationReportParameter compare) =>
        base.EqualKey(compare);
}