using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class RegulationShareLocalizer : LocalizerBase
{
    public RegulationShareLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string RegulationShare => FromCaller();
    public string RegulationShares => FromCaller();
    public string NotAvailable => FromCaller();
    public string MissingShares => FromCaller();

    public string ProviderTenant => FromCaller();
    public string ProviderRegulation => FromCaller();
    public string ConsumerTenant => FromCaller();
    public string ConsumerDivision => FromCaller();
}