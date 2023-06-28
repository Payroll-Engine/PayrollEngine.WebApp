using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class UserLocalizer : LocalizerBase
{
    public UserLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string User => FromCaller();
    public string Users => FromCaller();
    public string FirstName => FromCaller();
    public string LastName => FromCaller();
    public string UserType => FromCaller();
    public string Features => FromCaller();
    public string NotAvailable => FromCaller();

    public string UserSettings => FromCaller();

    public string ChangePasswordTitle => FromCaller();
    public string ChangePasswordButton => FromCaller();
    public string ShowPassword => FromCaller();
    public string ExistingPassword => FromCaller();
    public string ExistingPasswordHelp => FromCaller();
    public string NewPassword => FromCaller();
    public string NewPasswordHelp => FromCaller();
    public string NewPasswordRepeat => FromCaller();
    public string NewPasswordRepeatHelp => FromCaller();
}