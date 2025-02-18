using System;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model regulation report parameter
/// </summary>
public class RegulationReportParameter : ReportParameter, IRegulationItem, IKeyEquatable<RegulationReportParameter>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationReportParameter()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    private RegulationReportParameter(RegulationReportParameter copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    protected RegulationReportParameter(ReportParameter copySource) :
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
    [JsonIgnore]
    public RegulationItemType ItemType => RegulationItemType.ReportParameter;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => Name;
    
    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey => Parent?.InheritanceKey;

    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) => 
        Enum.GetName(typeof(ValueType), ValueType);

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

    /// <inheritdoc />
    public bool EqualKey(RegulationReportParameter compare) =>
        base.EqualKey(compare);
}