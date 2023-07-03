using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrollLayerLocalizer : LocalizerBase
{
    public PayrollLayerLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

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