using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class EmployeeLocalizer : LocalizerBase
{
    public EmployeeLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Employee => PropertyValue();
    public string Employees => PropertyValue();
    public string NotAvailable => PropertyValue();
    public string FirstName => PropertyValue();
    public string LastName => PropertyValue();
    public string CultureHelp => PropertyValue();
    public string CalendarHelp => PropertyValue();
    public string MissingEmployees => PropertyValue();
    public string OneEmployee => PropertyValue();

    public string AllEmployees(int count) =>
        FormatValue(PropertyValue(), nameof(count), count);
    public string SelectedEmployees(int selected, int total) =>
        FormatValue(PropertyValue(), nameof(selected), selected, nameof(total), total);
}