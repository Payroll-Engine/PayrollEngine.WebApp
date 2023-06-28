using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrollLayerLocalizer : LocalizerBase
{
    public PayrollLayerLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string PayrollLayer => FromCaller();
    public string PayrollLayers => FromCaller();
    public string NotAvailable => FromCaller();

    public string Level => FromCaller();
    public string Priority => FromCaller();
    public string DefaultLevel => FromCaller();

    public string NextLevelHelp => FromCaller();
    public string NextPriorityHelp => FromCaller();

    public string RuleLevelMin => FromCaller();
    public string RulePriorityMin => FromCaller();
    public string InvalidPayrollRegulation => FromCaller();
    public string InvalidBaseRegulation => FromCaller();

    public string MissingRegulation(string name) =>
        string.Format(FromCaller(), name);
    public string MissingBaseRegulation(string name) =>
        string.Format(FromCaller(), name);
}