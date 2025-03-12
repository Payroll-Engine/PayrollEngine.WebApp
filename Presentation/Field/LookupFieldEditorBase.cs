using System.Linq;
using System.Collections.Generic;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Field;

/// <summary>
/// Base class for lookup editor
/// </summary>
public abstract class LookupFieldEditorBase : FieldEditorBase
{
    /// <summary>
    /// Lookup value
    /// </summary>
    protected IEnumerable<LookupObject> LookupValues =>
        Field.LookupValues;

    /// <summary>
    /// Test for disabled values
    /// </summary>
    protected bool ValuesDisabled =>
        Disabled || !LookupValues.Any();

    /// <summary>
    /// Value field name
    /// </summary>
    protected string ValueFieldName =>
        Field.LookupSettings.ValueFieldName;

    /// <summary>
    /// Text field name
    /// </summary>
    protected string TextFieldName =>
        Field.LookupSettings.TextFieldName;
}