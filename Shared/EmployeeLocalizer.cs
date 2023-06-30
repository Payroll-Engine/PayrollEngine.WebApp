using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class EmployeeLocalizer : LocalizerBase
{
    public EmployeeLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Employee => FromCaller();
    public string Employees => FromCaller();
    public string NotAvailable => FromCaller();
    public string FirstName => FromCaller();
    public string LastName => FromCaller();
    public string CultureHelp => FromCaller();
    public string CalendarHelp => FromCaller();
    public string MissingEmployees => FromCaller();
    public string OneEmployee => FromCaller();

    public string AllEmployees(int count) =>
        string.Format(FromCaller(), count);
    public string SelectedEmployees(int selected, int total) =>
        string.Format(FromCaller(), selected, total);
}