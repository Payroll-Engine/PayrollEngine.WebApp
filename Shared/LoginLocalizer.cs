using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class LoginLocalizer : LocalizerBase
{
    public LoginLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Login => FromCaller();
    public string MissingBackendService => FromCaller();
    public string NewPasswordTitle => FromCaller();
    public string Password => FromCaller();
    public string PasswordInfo => FromCaller();
    public string InvalidPassword => FromCaller();
    public string InvalidPasswordConfirmation => FromCaller();
    public string PasswordChangeError => FromCaller();
    public string TenantHelp => FromCaller();
    public string UserHelp => FromCaller();
    public string TenantReadError=> FromCaller();
    public string UserReadError=> FromCaller();

    public string UnknownUser(string identifier) =>
        string.Format(FromCaller(), identifier);
}