﻿using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ReportQueryLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string ReportQuery => PropertyValue();
    public string ReportQueries => PropertyValue();
    public string NotAvailable => PropertyValue();
}