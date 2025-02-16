using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class RegulationShareLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string RegulationShare => PropertyValue();
    public string RegulationShares => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string MissingTenants => PropertyValue();
    public string MissingDivisions => PropertyValue();
    public string MissingSharedRegulations => PropertyValue();
    public string MissingShares => PropertyValue();

    public string ProviderTenant => PropertyValue();
    public string ProviderRegulation => PropertyValue();
    public string ConsumerTenant => PropertyValue();
    public string ConsumerDivision => PropertyValue();
}