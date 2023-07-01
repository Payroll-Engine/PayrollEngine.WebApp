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
    /// Field culture
    /// </summary>
    string Culture { get; }

    /// <summary>
    /// Get the localized name
    /// </summary>
    /// <param name="culture">The culture</param>
    /// <returns>The culture name</returns>
    string GetLocalizedName(string culture);

    /// <summary>
    /// Get the localized description
    /// </summary>
    /// <param name="culture">The culture</param>
    /// <returns>The culture description</returns>
    string GetLocalizedDescription(string culture);

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