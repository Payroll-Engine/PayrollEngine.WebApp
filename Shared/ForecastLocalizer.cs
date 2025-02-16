using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class ForecastLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Forecast => PropertyValue();
    public string Forecasts => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Name => PropertyValue();
    public string PayrunJob => PropertyValue();
    public string JobName => PropertyValue();
    public string JobPeriod => PropertyValue();
    public string JobHistory => PropertyValue();

    public string StartForecastPayrun => PropertyValue();

    public string CopyToForecast => PropertyValue();
    public string Copy => PropertyValue();
    public string CopyQuery => PropertyValue();

    public string JobNotSupported => PropertyValue();
}