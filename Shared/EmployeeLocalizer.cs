using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class EmployeeLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
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