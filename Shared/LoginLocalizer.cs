using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LoginLocalizer : LocalizerBase
{
    public LoginLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Login => PropertyValue();
    public string MissingBackendService => PropertyValue();
    public string NewPasswordTitle => PropertyValue();
    public string Password => PropertyValue();
    public string PasswordInfo => PropertyValue();
    public string InvalidPassword => PropertyValue();
    public string InvalidPasswordConfirmation => PropertyValue();
    public string PasswordChangeError => PropertyValue();
    public string TenantHelp => PropertyValue();
    public string UserHelp => PropertyValue();
    public string TenantReadError=> PropertyValue();
    public string UserReadError=> PropertyValue();

    public string UnknownUser(string user) =>
        FormatValue(PropertyValue(), nameof(user), user);
}