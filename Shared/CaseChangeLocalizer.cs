using Microsoft.Extensions.Localization;

namespace PayrollEngine.WebApp.Shared;

public class CaseChangeLocalizer : LocalizerBase
{
    public CaseChangeLocalizer(IStringLocalizerFactory factory) :
        base(factory)
    {
    }

    public string CaseChange => FromCaller();
    public string CaseChanges => FromCaller();
    public string NotAvailable => FromCaller();

    public string Undo => FromCaller();
    public string Reason => FromCaller();
    public string UndoCaseChange => FromCaller();
    public string EmptyCaseChange => FromCaller();

    public string UndoError(string caseName) =>
        string.Format(FromCaller(), caseName);
    public string UndoSuccess(string caseName) =>
        string.Format(FromCaller(), caseName);
    public string UndoQuery(string caseName) =>
        string.Format(FromCaller(), caseName);
    public string CaseChangeGroup(object text) =>
        string.Format(FromCaller(), text);
    public string UndoCancelGroup(string cancelDate) =>
        string.Format(FromCaller(), cancelDate);
}