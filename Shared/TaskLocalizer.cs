using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class TaskLocalizer : LocalizerBase
{
    public TaskLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string Task => FromCaller();
    public string Tasks => FromCaller();
    public string NotAvailable => FromCaller();

    public string Complete => FromCaller();
    public string CompleteToggle => FromCaller();
    public string CompleteToggleHelp => FromCaller();

    public string Category => FromCaller();
    public string Instruction => FromCaller();
    public string ScheduledDate => FromCaller();
    public string CompletedDate => FromCaller();
}