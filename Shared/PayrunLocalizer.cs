using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunLocalizer : LocalizerBase
{
    public PayrunLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Payrun => FromCaller();
    public string Payruns => FromCaller();
    public string NotAvailable => FromCaller();

    public string Parameters => FromCaller();
    public string Expressions => FromCaller();

    public string DefaultReason => FromCaller();
    public string RetroTimeType => FromCaller();

    public string StartExpression => FromCaller();
    public string EmployeeAvailableExpression => FromCaller();
    public string EmployeeStartExpression => FromCaller();
    public string EmployeeEndExpression => FromCaller();
    public string WageTypeAvailableExpression => FromCaller();
    public string EndExpression => FromCaller();

    public string UnknownPayrun(string name) =>
        string.Format(FromCaller(), name);
}