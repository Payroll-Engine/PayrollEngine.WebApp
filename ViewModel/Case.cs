using PayrollEngine.Client.Model;
using System;

namespace PayrollEngine.WebApp.ViewModel;

public class Case : Client.Model.Case, IViewModel,
    IViewAttributeObject, IKeyEquatable<Case>
{
    // ReSharper disable once MemberCanBeProtected.Global
    public Case()
    {
    }

    protected Case(Case copySource) :
        base(copySource)
    {
        CaseSlot = copySource.CaseSlot;
    }

    protected Case(Client.Model.Case copySource) :
        base(copySource)
    {
    }

    public string GetDisplayName(string culture)
    {
        var displayName = culture.GetLocalization(NameLocalizations, Name);
        if (!string.IsNullOrWhiteSpace(CaseSlot))
        {
            displayName = $"{displayName} {CaseSlot}";
        }
        return displayName;
    }

    /// <summary>
    /// The case slot
    /// </summary>
    public string CaseSlot { get; }

    public string GetLocalizedName(string culture) =>
        culture.GetLocalization(NameLocalizations, Name);

    public string GetLocalizedDescription(string culture) =>
        culture.GetLocalization(DescriptionLocalizations, Description);

    public string GetLocalizedDefaultReason(string culture) =>
        culture.GetLocalization(DefaultReasonLocalizations, DefaultReason);

    public bool IsMatching(string search, string culture)
    {
        var name = culture.GetLocalization(NameLocalizations, Name);
        if (name.Contains(search, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }
        if (!string.IsNullOrWhiteSpace(Description))
        {
            var description = culture.GetLocalization(DescriptionLocalizations, Name);
            if (description.Contains(search, StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(Case compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public virtual bool Equals(IViewModel compare) =>
        Equals(compare as Case);

    public bool EqualKey(Case compare) =>
        base.EqualKey(compare);

    #region Attributes

    /// <inheritdoc />
    public string GetStringAttribute(string name) =>
        Attributes?.GetStringAttributeValue(name);

    /// <inheritdoc />
    public decimal GetNumericAttribute(string name) =>
        Attributes?.GetDecimalAttributeValue(name) ?? 0;

    /// <inheritdoc />
    public bool GetBooleanAttribute(string name) =>
        Attributes?.GetBooleanAttributeValue(name) ?? false;

    #endregion
}