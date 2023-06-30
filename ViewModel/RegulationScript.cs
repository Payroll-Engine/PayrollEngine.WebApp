using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

public class RegulationScript : Script, IRegulationItem, IKeyEquatable<RegulationScript>
{
    public RegulationScript()
    {
    }

    private RegulationScript(RegulationScript copySource) :
        base(copySource)
    {
        CopyTool.CopyProperties(copySource, this);
    }

    protected RegulationScript(Script copySource) :
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
    public Dictionary<string, object> Attributes { get; set; }

    /// <inheritdoc />
    [JsonIgnore]
    public RegulationItemType ItemType => RegulationItemType.Script;

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
    public string GetAdditionalInfo(Localizer localizer)
    {
        if (FunctionTypes == null || !FunctionTypes.Any())
        {
            return string.Empty;
        }
        return FunctionTypes.Count == 1 ?
            localizer.Script.SingleFunction : localizer.Script.FunctionCount(FunctionTypes.Count);
    }

    /// <inheritdoc />
    public IRegulationItem Parent { get; set; }

    /// <inheritdoc />
    public IRegulationItem Clone() =>
        new RegulationScript(this);

    /// <inheritdoc />
    public void ApplyInheritanceKeyTo(IRegulationItem target)
    {
        if (target is RegulationScript script)
        {
            script.Name = Name;
        }
    }

    #endregion

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(RegulationScript compare) =>
        base.Equals(compare) &&
        CompareTool.EqualProperties(this, compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as RegulationScript);

    public bool EqualKey(RegulationScript compare) =>
        base.EqualKey(compare);
}