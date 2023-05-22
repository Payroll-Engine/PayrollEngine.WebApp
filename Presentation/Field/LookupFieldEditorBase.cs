using System.Collections.Generic;
using System.Linq;
using PayrollEngine.WebApp.ViewModel;

namespace PayrollEngine.WebApp.Presentation.Field;

public abstract class LookupFieldEditorBase : FieldEditorBase
{
    protected IEnumerable<LookupObject> LookupValues =>
        Field.LookupValues;

    protected bool ValuesDisabled =>
        Disabled || !LookupValues.Any();

    protected string ValueFieldName =>
        Field.LookupSettings.ValueFieldName;

    protected string TextFieldName =>
        Field.LookupSettings.TextFieldName;
}