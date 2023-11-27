using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class TaskLocalizer(IStringLocalizerFactory factory) : LocalizerBase(factory)
{
    public string Task => PropertyValue();
    public string Tasks => PropertyValue();
    public string NotAvailable => PropertyValue();

    public string Complete => PropertyValue();
    public string CompleteToggle => PropertyValue();
    public string CompleteToggleHelp => PropertyValue();

    public string Category => PropertyValue();
    public string Instruction => PropertyValue();
    public string ScheduledDate => PropertyValue();
    public string CompletedDate => PropertyValue();
}