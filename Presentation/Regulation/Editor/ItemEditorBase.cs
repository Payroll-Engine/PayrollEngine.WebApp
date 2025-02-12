using System;
using Microsoft.AspNetCore.Components;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Regulation.Editor;

public abstract class ItemEditorBase<T> : ComponentBase, IRegulationItemValidator
    where T : class, IRegulationItem
{
    string IRegulationItemValidator.Validate(IRegulationItem item)
    {
        var editorItem = item as T;
        if (editorItem == null)
        {
            throw new ArgumentException($"Invalid regulation item type {item} ({item.ItemType})");
        }
        return OnValidate(editorItem);
    }

    protected virtual string OnValidate(T caseRelation) => null;
}