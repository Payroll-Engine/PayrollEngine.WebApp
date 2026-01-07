using System.Collections.Generic;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Regulation item
/// </summary>
public interface IRegulationItem : IViewModel
{
    /// <summary>
    /// Regulation name
    /// </summary>
    string RegulationName { get; set; }

    /// <summary>
    /// Base item
    /// </summary>
    IRegulationItem BaseItem { get; set; }

    /// <summary>
    /// Attributes
    /// </summary>
    Dictionary<string, object> Attributes { get; set; }

    /// <summary>
    /// Item type
    /// </summary>
    RegulationItemType ItemType { get; }

    /// <summary>
    /// Item type name
    /// </summary>
    string ItemTypeName => ItemType.ToString().ToPascalSentence();

    /// <summary>
    /// Inheritance type
    /// </summary>
    RegulationInheritanceType InheritanceType { get; set; }

    /// <summary>
    /// Inheritance key
    /// </summary>
    string InheritanceKey => Name;

    /// <summary>
    /// Parent item inheritance key
    /// </summary>
    string ParentInheritanceKey => null;

    /// <summary>
    /// Item name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Item description
    /// </summary>
    string Description => null;

    /// <summary>
    /// Action count
    /// </summary>
    int ActionCount => 0;

    /// <summary>
    /// Expression count
    /// </summary>
    int ExpressionCount => 0;

    /// <summary>
    /// Attribute count
    /// </summary>
    int AttributeCount => Attributes?.Count ?? 0;

    /// <summary>
    /// Get group count
    /// </summary>
    int GetGroupCount(string groupName) => 0;

    /// <summary>
    /// Get additional item info
    /// </summary>
    /// <param name="localizer">Localizer</param>
    string GetAdditionalInfo(Localizer localizer);

    /// <summary>
    /// Parent regulation item
    /// </summary>
    IRegulationItem Parent { get; set; }

    /// <summary>
    /// Test for child item
    /// </summary>
    bool IsChildItem =>
        Parent != null;

    /// <summary>
    /// Clone item
    /// </summary>
    IRegulationItem Clone();

    /// <summary>
    /// Apply the inheritance key
    /// </summary>
    /// <param name="target">Target regulation item</param>
    void ApplyInheritanceKeyTo(IRegulationItem target);
}