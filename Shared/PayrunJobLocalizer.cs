using System.Globalization;
using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunJobLocalizer(IStringLocalizerFactory factory, CultureInfo culture) : 
    LocalizerBase(factory, culture: culture)
{
    public string PayrunJob => PropertyValue();
    public string PayrunJobs => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string JobResults => PropertyValue();
    public string Legal => PropertyValue();

    public string Copy => PropertyValue();

    public string JobName => PropertyValue();
    public string JobStatus => PropertyValue();
    public string JobReason => PropertyValue();
    public string JobPeriod => PropertyValue();
    public string JobHistory => PropertyValue();
    public string EvaluationDate => PropertyValue();

    public string StartPayrun => PropertyValue();
    public string ShowJobDetails => PropertyValue();
    public string New => PropertyValue();

    public string JobDate => PropertyValue();
    public string JobValidationFailed => PropertyValue();

    public string MissingJobPeriod => PropertyValue();
    public string MissingJobReason => PropertyValue();

    public string JobStart => PropertyValue();
    public string JobProcessing(int percent) =>
        FormatValue(PropertyValue(), nameof(percent), percent);
    public string JobFailed => PropertyValue();
    public string JobCompleted => PropertyValue();

    public string DefaultReason(PayrunJobStatus status) =>
        FormatValue(PropertyValue(), nameof(status), Enum(status));
    public string StatusChanged(PayrunJobStatus status) =>
        FormatValue(PropertyValue(), nameof(status), Enum(status));
    public object ChangeStatusQuery(string source, string target) =>
        FormatValue(PropertyValue(), nameof(source), source, 
            nameof(target), target);
}