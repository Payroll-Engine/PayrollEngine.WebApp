using System.Collections.Generic;
using PayrollEngine.Client.Model;

namespace PayrollEngine.WebApp.ViewModel;

public interface IFieldObject : IVariantValue, IAttributeObject
{
    /// <summary>
    /// Field name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Get the localized name
    /// </summary>
    /// <param name="language">The language</param>
    /// <returns>The language name</returns>
    string GetLocalizedName(Language language);

    /// <summary>
    /// Get the localized description
    /// </summary>
    /// <param name="language">The language</param>
    /// <returns>The language description</returns>
    string GetLocalizedDescription(Language language);

    /// <summary>
    /// The value formatter
    /// </summary>
    public IValueFormatter ValueFormatter { get; }

    /// <summary>
    /// The lookup settings
    /// </summary>
    public LookupSettings LookupSettings { get; }

    /// <summary>
    /// The lookup values
    /// </summary>
    public List<LookupObject> LookupValues { get; }

    /// <summary>
    /// The value mandatory state
    /// </summary>
    bool ValueMandatory { get; }

    /// <summary>
    /// Test if the value is valid
    /// </summary>
    bool IsValidValue();
}