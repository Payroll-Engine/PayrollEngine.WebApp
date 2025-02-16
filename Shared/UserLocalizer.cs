using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class UserLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string User => PropertyValue();
    public string Users => PropertyValue();
    public string FirstName => PropertyValue();
    public string LastName => PropertyValue();
    public string UserType => PropertyValue();
    public string Features => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string UserSettings => PropertyValue();

    public string ChangePasswordTitle => PropertyValue();
    public string ChangePasswordButton => PropertyValue();
    public string ShowPassword => PropertyValue();
    public string ExistingPassword => PropertyValue();
    public string ExistingPasswordHelp => PropertyValue();
    public string NewPassword => PropertyValue();
    public string NewPasswordHelp => PropertyValue();
    public string NewPasswordRepeat => PropertyValue();
    public string NewPasswordRepeatHelp => PropertyValue();
    public string PasswordChanged => PropertyValue();
}