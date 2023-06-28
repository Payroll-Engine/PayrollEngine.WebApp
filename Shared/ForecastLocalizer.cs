using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class ForecastLocalizer : LocalizerBase
{
    public ForecastLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Forecast => FromCaller();
    public string Forecasts => FromCaller();
    public string NotAvailable => FromCaller();

    public string Name => FromCaller();
    public string PayrunJob => FromCaller();
    public string JobName => FromCaller();
    public string JobPeriod => FromCaller();
    public string JobHistory => FromCaller();

    public string StartForecastPayrun => FromCaller();

    public string CopyToForecast => FromCaller();
    public string Copy => FromCaller();
    public string CopyQuery => FromCaller();

    public string JobNotSupported => FromCaller();
}