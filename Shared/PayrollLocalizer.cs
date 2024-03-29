﻿using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrollLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Payroll => PropertyValue();
    public string Payrolls => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string WageTypePeriod => PropertyValue();
    public string WageTypeRetro => PropertyValue();
    public string CollectorRetro => PropertyValue();
}