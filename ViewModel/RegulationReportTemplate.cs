using System;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationReportTemplate : ReportTemplate, IRegulationItem, IEquatable<RegulationReportTemplate>
{
    public RegulationReportTemplate()
    {
    }

    public RegulationReportTemplate(RegulationReportTemplate copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    protected RegulationReportTemplate(ReportTemplate copySource) :
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
    public RegulationItemType ItemType => RegulationItemType.ReportTemplate;

    /// <inheritdoc />
    [JsonIgnore]
    public string InheritanceKey => Name;

    /// <inheritdoc />
    [JsonIgnore]
    public string ParentInheritanceKey => Parent?.InheritanceKey;

    /// <inheritdoc />
    [JsonIgnore]
    public string Description => null;

    /// <inheritdoc />
    public string GetAdditionalInfo(Localizer localizer) => 
        Culture;

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationReportTemplate(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationReportTemplate reportTemplate)
        {
            reportTemplate.Name = Name;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationReportTemplate compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <inheritdoc />
    public bool Equals(IViewModel compare) =>
        Equals(compare as RegulationReportTemplate);
}