using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LoginLocalizer : LocalizerBase
{
    public LoginLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Login => FromCaller();

    public string NewPasswordTitle => FromCaller();
    public string Password => FromCaller();
    public string PasswordInfo => FromCaller();

    public string TenantHelp => FromCaller();
    public string UserHelp => FromCaller();
}