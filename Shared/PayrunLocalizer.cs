using Microsoft.Extensions.Localization;
using System.Globalization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string Payrun => PropertyValue();
    public string Payruns => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Parameters => PropertyValue();
    public string Expressions => PropertyValue();

    public string DefaultReason => PropertyValue();
    public string RetroTimeType => PropertyValue();

    public string StartExpression => PropertyValue();
    public string EmployeeAvailableExpression => PropertyValue();
    public string EmployeeStartExpression => PropertyValue();
    public string EmployeeEndExpression => PropertyValue();
    public string WageTypeAvailableExpression => PropertyValue();
    public string EndExpression => PropertyValue();

    public string UnknownPayrun(string payrun) =>
        FormatValue(PropertyValue(), nameof(payrun), payrun);
}