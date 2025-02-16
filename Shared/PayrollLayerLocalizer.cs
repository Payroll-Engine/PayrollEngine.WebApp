using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class PayrollLayerLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string PayrollLayer => PropertyValue();
    public string PayrollLayers => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Level => PropertyValue();
    public string Priority => PropertyValue();
    public string DefaultLevel => PropertyValue();

    public string NextLevelHelp => PropertyValue();
    public string NextPriorityHelp => PropertyValue();

    public string RuleLevelMin => PropertyValue();
    public string RulePriorityMin => PropertyValue();
    public string InvalidPayrollRegulation => PropertyValue();
    public string InvalidBaseRegulation => PropertyValue();

    public string MissingRegulation(string regulation) =>
        FormatValue(PropertyValue(), nameof(regulation), regulation);
    public string MissingBaseRegulation(string regulation) =>
        FormatValue(PropertyValue(), nameof(regulation), regulation);
}