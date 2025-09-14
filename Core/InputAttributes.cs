namespace PayrollEngine.WebApp;

/// <summary>
/// Application input attributes
/// </summary>
public static class InputAttributes
{
    public static readonly string Prefix = "input.";

    #region Transient

    // transient during object edit
    public static readonly string EditInfo = $"{Prefix}editInfo";
    public static readonly string Validity = $"{Prefix}validity";

    #endregion

    #region Case General

    internal static readonly string Icon = $"{Prefix}icon";
    internal static readonly string Priority = $"{Prefix}priority";

    #endregion

    #region Case Field General

    public static readonly string Group = $"{Prefix}group";
    public static readonly string Separator = $"{Prefix}separator";
    public static readonly string Hidden = $"{Prefix}hidden";
    public static readonly string HiddenName = $"{Prefix}hiddenName";
    public static readonly string FieldLayout = $"{Prefix}fieldLayout";
    public static readonly string ShowDescription = $"{Prefix}showDescription";
    public static readonly string Variant = $"{Prefix}variant";

    #endregion

    #region Case Field Start

    public static readonly string StartLabel = $"{Prefix}startLabel";
    public static readonly string StartHelp = $"{Prefix}startHelp";
    public static readonly string StartRequired = $"{Prefix}startRequired";
    public static readonly string StartReadOnly = $"{Prefix}startReadOnly";
    public static readonly string StartHidden = $"{Prefix}startHidden";
    public static readonly string StartFormat = $"{Prefix}startFormat";
    public static readonly string StartPickerOpen = $"{Prefix}startPickerOpen";
    public static readonly string StartPickerType = $"{Prefix}startPickerType";

    #endregion

    #region Case Field End

    public static readonly string EndLabel = $"{Prefix}endLabel";
    public static readonly string EndHelp = $"{Prefix}endHelp";
    public static readonly string EndRequired = $"{Prefix}endRequired";
    public static readonly string EndReadOnly = $"{Prefix}endReadOnly";
    public static readonly string EndHidden = $"{Prefix}endHidden";
    public static readonly string EndFormat = $"{Prefix}endFormat";
    public static readonly string EndPickerOpen = $"{Prefix}endPickerOpen";
    public static readonly string EndPickerType = $"{Prefix}endPickerType";

    #endregion

    #region Case Field Value

    public static readonly string ValueLabel = $"{Prefix}valueLabel";
    public static readonly string ValueAdornment = $"{Prefix}valueAdornment";
    public static readonly string ValueHelp = $"{Prefix}valueHelp";
    public static readonly string ValueMask = $"{Prefix}valueMask";
    public static readonly string ValueRequired = $"{Prefix}valueRequired";
    public static readonly string ValueReadOnly = $"{Prefix}valueReadOnly";
    public static readonly string ValuePickerOpen = $"{Prefix}valuePickerOpen";
    public static readonly string ValuePickerStatic = $"{Prefix}valuePickerStatic";
    public static readonly string ValueTimePicker = $"{Prefix}valueTimePicker";
    public static readonly string MinValue = $"{Prefix}minValue";
    public static readonly string MaxValue = $"{Prefix}maxValue";
    public static readonly string StepSize = $"{Prefix}stepSize";
    public static readonly string Format = $"{Prefix}format";
    public static readonly string LineCount = $"{Prefix}lineCount";
    public static readonly string MaxLength = $"{Prefix}maxLength";
    public static readonly string Check = $"{Prefix}check";
    public static readonly string ValueHistory = $"{Prefix}valueHistory";
    // no value picker type: the case field value-type pre defines the picker type

    #endregion

    #region Case Field Attachments

    public static readonly string Attachment = $"{Prefix}attachment";
    public static readonly string AttachmentExtensions = $"{Prefix}attachmentExtensions";

    #endregion

    #region List

    public static readonly string List = $"{Prefix}list";
    public static readonly string ListValues = $"{Prefix}listValues";
    public static readonly string ListSelection = $"{Prefix}listSelection";

    #endregion

}