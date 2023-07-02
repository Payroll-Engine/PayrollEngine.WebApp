using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseLocalizer : LocalizerBase
{
    public CaseLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Case => FromCaller();
    public string Cases => FromCaller();
    public string NotAvailable => FromCaller();

    public string CaseType => FromCaller();
    public string DefaultReason => FromCaller();
    public string CancellationType => FromCaller();
    public string BaseCase => FromCaller();
    public string BaseCaseField => FromCaller();
    public string BaseCaseFields => FromCaller();
    public string Slots => FromCaller();

    public string AvailableExpression => FromCaller();
    public string BuildExpression => FromCaller();
    public string ValidateExpression => FromCaller();
    public string AvailableActions => FromCaller();
    public string BuildActions => FromCaller();
    public string ValidateActions => FromCaller();

    public string GlobalCases => FromCaller();
    public string NationalCases => FromCaller();
    public string CompanyCases => FromCaller();
    public string EmployeeCases => FromCaller();

    public string UndoCase => FromCaller();
    public string ChangeHistory => FromCaller();
    public string AvailableCases => FromCaller();

    public string SelectCase => FromCaller();
    public string CaseWithoutFields => FromCaller();
    public string Validation => FromCaller();

    public string StartCase(string caseName) =>
        string.Format(FromCaller(), caseName);
    public string SearchCase(string caseName) =>
        string.Format(FromCaller(), caseName);
}