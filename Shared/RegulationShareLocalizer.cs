﻿using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class RegulationShareLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string RegulationShare => PropertyValue();
    public string RegulationShares => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string MissingShares => PropertyValue();

    public string ProviderTenant => PropertyValue();
    public string ProviderRegulation => PropertyValue();
    public string ConsumerTenant => PropertyValue();
    public string ConsumerDivision => PropertyValue();
}