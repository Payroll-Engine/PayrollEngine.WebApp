using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ForecastLocalizer : LocalizerBase
{
    public ForecastLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

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