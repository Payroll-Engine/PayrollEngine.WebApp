using System.Globalization;
using Microsoft.Extensions.Localization;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.Presentation;

/// <inheritdoc />
public class LocalizerService : ILocalizerService
{
    private UserSession UserSession { get; }
    private ICultureService CultureService { get; }
    private IStringLocalizerFactory Factory { get; }
    private readonly Localizer defaultLocalizer;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="userSession">USer session</param>
    /// <param name="cultureService">Culture service</param>
    /// <param name="factory">String localizer factory</param>
    public LocalizerService(UserSession userSession, ICultureService cultureService, IStringLocalizerFactory factory)
    {
        UserSession = userSession;
        CultureService = cultureService;
        Factory = factory;

        // build localizer
        if (!BuildLocalizer())
        {
            defaultLocalizer = new Localizer(factory, CultureInfo.CurrentUICulture);
        }
    }

    private Localizer localizer;
    /// <inheritdoc />
    public Localizer Localizer
    {
        get
        {
            if (localizer == null)
            {
                BuildLocalizer();
            }
            return localizer ?? defaultLocalizer;
        }
    }

    private bool BuildLocalizer()
    {
        if (string.IsNullOrWhiteSpace(UserSession.User?.Culture))
        {
            return false;
        }

        var culture = CultureService.GetCulture(UserSession.User.Culture).CultureInfo;
        localizer = new Localizer(Factory, culture: culture);
        return true;
    }
}