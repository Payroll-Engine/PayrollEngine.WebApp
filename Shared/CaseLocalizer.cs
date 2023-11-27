using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Case => PropertyValue();
    public string Cases => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string CaseType => PropertyValue();
    public string DefaultReason => PropertyValue();
    public string CancellationType => PropertyValue();
    public string BaseCase => PropertyValue();
    public string BaseCaseField => PropertyValue();
    public string BaseCaseFields => PropertyValue();
    public string Slots => PropertyValue();

    public string AvailableExpression => PropertyValue();
    public string BuildExpression => PropertyValue();
    public string ValidateExpression => PropertyValue();
    public string AvailableActions => PropertyValue();
    public string BuildActions => PropertyValue();
    public string ValidateActions => PropertyValue();

    public string GlobalCases => PropertyValue();
    public string NationalCases => PropertyValue();
    public string CompanyCases => PropertyValue();
    public string EmployeeCases => PropertyValue();

    public string UndoCase => PropertyValue();
    public string ChangeHistory => PropertyValue();
    public string AvailableCases => PropertyValue();

    public string SelectCase => PropertyValue();
    public string CaseWithoutFields => PropertyValue();
    public string Validation => PropertyValue();
    public string ValidationFailed => PropertyValue();

    public string SubmitCase => PropertyValue();
    public string SubmitForecastCase => PropertyValue();

    public string StartCase(string @case) =>
        FormatValue(PropertyValue(), nameof(@case), @case);
    public string SearchCase(string @case) =>
        FormatValue(PropertyValue(), nameof(@case), @case);
    public string CaseAdded(string @case) =>
        FormatValue(PropertyValue(), nameof(@case), @case);
    public string CaseIgnored(string @case) =>
        FormatValue(PropertyValue(), nameof(@case), @case);
    public string MissingCase(string name) =>
        FormatValue(PropertyValue(), nameof(name), name);
}