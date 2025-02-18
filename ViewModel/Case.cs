using System;
using System.Globalization;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model case
/// </summary>
public class Case : Client.Model.Case, IViewModel,
    IViewAttributeObject, IKeyEquatable<Case>
{
    /// <summary>
    /// Default constructor
    /// </summary>
    // ReSharper disable once MemberCanBeProtected.Global
    public Case()
    {
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    protected Case(Case copySource) :
        base(copySource)
    {
        CaseSlot = copySource.CaseSlot;
    }

    /// <summary>
    /// Base model constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    protected Case(Client.Model.Case copySource) :
        base(copySource)
    {
    }

    /// <summary>
    /// Get the display name
    /// </summary>
    /// <param name="culture">Culture</param>
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

    /// <summary>
    /// Get the localized name
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedName(string culture) =>
        culture.GetLocalization(NameLocalizations, Name);

    /// <summary>
    /// Get localized description
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedDescription(string culture) =>
        culture.GetLocalization(DescriptionLocalizations, Description);

    /// <summary>
    /// Get localized default change reason
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedDefaultReason(string culture) =>
        culture.GetLocalization(DefaultReasonLocalizations, DefaultReason);

    /// <summary>
    /// Test for matching case
    /// </summary>
    /// <param name="search">Search text</param>
    /// <param name="culture">Culture</param>
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

    private CasePriority? priority;
    /// <summary>
    /// Get case priority
    /// </summary>
    /// <param name="culture">Culture</param>
    public CasePriority GetPriority(CultureInfo culture)
    {
        priority ??= Attributes.GetPriority(culture) ?? CasePriority.Normal;
        return priority.Value;
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