using System;

namespace PayrollEngine.WebApp.ViewModel;

public class CaseValueSetup : Client.Model.CaseValueSetup, IViewModel, IEquatable<CaseValueSetup>
{
    public IValueFormatter ValueFormatter { get; }

    public CaseValueSetup(string caseName, CaseFieldSet caseField, IValueFormatter valueFormatter)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (caseField == null)
        {
            throw new ArgumentNullException(nameof(caseField));
        }
        ValueFormatter = valueFormatter ?? throw new ArgumentNullException(nameof(valueFormatter));

        // copy values
        CaseName = caseName;
        CaseFieldName = caseField.Name;
        CaseSlot = caseField.CaseSlot;
        ValueType = caseField.ValueType;
        Value = caseField.Value;
        CancellationDate = caseField.CancellationDate;
        Start = caseField.Start;
        End = caseField.End;
        Attributes = caseField.ValueAttributes;
    }

    public CaseValueSetup(Client.Model.CaseValueSetup copySource, IValueFormatter valueFormatter) :
        base(copySource)
    {
        ValueFormatter = valueFormatter ?? throw new ArgumentNullException(nameof(valueFormatter));

        // Special case: new Value function to set FormattedValue is never called in base constructor
        FormattedValue = ValueFormatter.ToString(copySource.Value, ValueType);
    }

    /// <summary>
    /// The case value (JSON format)
    /// </summary>
    public new string Value
    {
        get => base.Value;
        set
        {
            if (value != base.Value)
            {
                base.Value = value;
                FormattedValue = ValueFormatter.ToString(value, ValueType);
            }
        }
    }

    /// <summary>
    /// The formatted case value
    /// </summary>
    public string FormattedValue { get; private set; }

    public void ChangeValue(string newValue) =>
        Value = newValue;

    /// <summary>
    /// The case change description
    /// </summary>
    public string StringDescription =>
        $"{Created}: {CaseName}.{CaseFieldName}={Value}";

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(CaseValueSetup compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as CaseValueSetup);
}