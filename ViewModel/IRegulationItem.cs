using System.Collections.Generic;

namespace PayrollEngine.WebApp.ViewModel;

public interface IRegulationItem : IViewModel
{
    int RegulationId { get; set; }
    string RegulationName { get; set; }

    IRegulationItem BaseItem { get; set; }
    Dictionary<string, object> Attributes { get; set; }

    RegulationItemType ItemType { get; }
    string ItemName => ItemType.ToString().ToPascalSentence();

    RegulationInheritanceType InheritanceType { get; set; }
    string InheritanceKey { get; }
    string ParentInheritanceKey { get; }

    string Name { get; }
    string Description { get; }
    string AdditionalInfo { get; }

    // child object
    IRegulationItem Parent { get; set; }
    bool IsChildItem =>
        Parent != null;

    IRegulationItem Clone();
    void ApplyInheritanceKeyTo(IRegulationItem target);
}