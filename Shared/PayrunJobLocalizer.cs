using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class PayrunJobLocalizer : LocalizerBase
{
    public PayrunJobLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string PayrunJob => FromCaller();
    public string PayrunJobs => FromCaller();
    public string NotAvailable => FromCaller();

    public string JobResults => FromCaller();
    public string Legal => FromCaller();

    public string Copy => FromCaller();

    public string JobName => FromCaller();
    public string JobStatus => FromCaller();
    public string JobReason => FromCaller();
    public string JobPeriod => FromCaller();
    public string JobHistory => FromCaller();

    public string StartPayrun => FromCaller();
    public string ShowJobDetails => FromCaller();
    public string New => FromCaller();

    public string JobDate => FromCaller();
    public string JobValidationFailed => FromCaller();

    public string MissingJobPeriod => FromCaller();
    public string MissingJobReason => FromCaller();

    public string JobStart => FromCaller();
    public string JobExecuting => FromCaller();
    public string JobFailed => FromCaller();
    public string JobCompleted => FromCaller();

    public string DefaultReason(string status) =>
        string.Format(FromCaller(), status);
    public string StatusChanged(string status) =>
        string.Format(FromCaller(), status);
}