namespace PayrollEngine.WebApp;

public static class InputAttributes
{
    private static readonly string Prefix = "input.";

    // case general
    internal static readonly string Icon = $"{Prefix}icon";

    // case field general
    public static readonly string Hidden = $"{Prefix}hidden";
    public static readonly string ShowDescription = $"{Prefix}showDescription";

    // case field start
    public static readonly string StartLabel = $"{Prefix}startLabel";
    public static readonly string StartHelp = $"{Prefix}startHelp";
    public static readonly string StartRequired = $"{Prefix}startRequired";
    public static readonly string StartReadOnly = $"{Prefix}startReadOny";
    public static readonly string StartFormat = $"{Prefix}startFormat";
    public static readonly string StartPickerOpen = $"{Prefix}startPickerOpen";
    public static readonly string StartPickerType = $"{Prefix}startPickerType";

    // case field end
    public static readonly string EndLabel = $"{Prefix}endLabel";
    public static readonly string EndHelp = $"{Prefix}endHelp";
    public static readonly string EndRequired = $"{Prefix}endRequired";
    public static readonly string EndReadOnly = $"{Prefix}endReadOny";
    public static readonly string EndFormat = $"{Prefix}endFormat";
    public static readonly string EndPickerOpen = $"{Prefix}endPickerOpen";
    public static readonly string EndPickerType = $"{Prefix}endPickerType";

    // case field value
    public static readonly string ValueLabel = $"{Prefix}valueLabel";
    public static readonly string ValueAdornment = $"{Prefix}valueAdornment";
    public static readonly string ValueHelp = $"{Prefix}valueHelp";
    public static readonly string ValueMask = $"{Prefix}valueMask";
    public static readonly string ValueRequired = $"{Prefix}valueRequired";
    public static readonly string ValueReadOnly = $"{Prefix}valueReadOnly";
    public static readonly string ValuePickerOpen = $"{Prefix}valuePickerOpen";
    public static readonly string Culture = $"{Prefix}culture";
    public static readonly string MinValue = $"{Prefix}minValue";
    public static readonly string MaxValue = $"{Prefix}maxValue";
    public static readonly string StepSize = $"{Prefix}stepSize";
    public static readonly string Format = $"{Prefix}format";
    public static readonly string LineCount = $"{Prefix}lineCount";
    public static readonly string MaxLength = $"{Prefix}maxLength";
    public static readonly string Check = $"{Prefix}check";
    public static readonly string ValueHistory = $"{Prefix}valueHistory";
    // no value picker type: the case field value-type pre defines the picker type

    // case field attachments
    public static readonly string Attachment = $"{Prefix}attachment";
    public static readonly string AttachmentExtensions = $"{Prefix}attachmentExtensions";

    // list
    public static readonly string List = $"{Prefix}list";
    public static readonly string ListValues = $"{Prefix}listValues";
    public static readonly string ListSelection = $"{Prefix}listSelection";
}