using System;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// Case value setup
/// </summary>
public class CaseValueSetup : Client.Model.CaseValueSetup, IViewModel, IEquatable<CaseValueSetup>
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="caseName">Case name</param>
    /// <param name="caseField">Case field</param>
    public CaseValueSetup(string caseName, CaseFieldSet caseField)
    {
        if (string.IsNullOrWhiteSpace(caseName))
        {
            throw new ArgumentException(nameof(caseName));
        }
        if (caseField == null)
        {
            throw new ArgumentNullException(nameof(caseField));
        }

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

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    public CaseValueSetup(Client.Model.CaseValueSetup copySource) :
        base(copySource)
    {
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
            }
        }
    }

    /// <summary>
    /// Change the value
    /// </summary>
    /// <param name="newValue">New value</param>
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