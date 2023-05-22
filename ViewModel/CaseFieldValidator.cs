using System;
using System.Linq;

namespace PayrollEngine.WebApp.ViewModel;

public class CaseFieldValidator : ICaseFieldValidator
{
    private const string MissingValue = "Missing value";
    private const string MissingStart = "Missing start date";
    private const string MissingEnd = "Missing end date";
    private const string MissingAttachment = "Missing attachment";

    public CaseFieldSet CaseField { get; }

    public CaseFieldValidator(CaseFieldSet caseField)
    {
        CaseField = caseField ?? throw new ArgumentNullException(nameof(caseField));
    }


    /// <summary>
    /// Validate case field
    /// </summary>
    /// <returns>Valid state if the case field is complete, otherwise return list of broken validation rules</returns>
    public CaseObjectValidity Validate()
    {
        var validity = new CaseObjectValidity();
        if (CaseField.ValueType == ValueType.None)
        {
            // no values at all
            return validity;
        }

        var hasValue = CaseField.HasValue;

        // timeless
        if (CaseField.TimeType == CaseFieldTimeType.Timeless)
        {
            if (CaseField.ValueMandatory && !hasValue)
            {
                validity.AddRule(CaseField.Name, MissingValue);
            }
            return validity;
        }

        // all period time types
        var hasStart = CaseField.Start.HasValue;
        var hasEnd = CaseField.End.HasValue;

        // mandatory field with start and value
        if (CaseField.ValueMandatory)
        {
            // start
            if (!hasStart)
            {
                validity.AddRule(CaseField.Name, MissingStart);
                return validity;
            }
            // end
            if (CaseField.EndMandatory && !hasEnd)
            {
                validity.AddRule(CaseField.Name, MissingEnd);
                return validity;
            }
            // value
            if (!hasValue)
            {
                validity.AddRule(CaseField.Name, MissingValue);
                return validity;
            }
        }
        else
        {
            // optional field value
            // start
            if (!hasStart && (hasEnd || hasValue))
            {
                validity.AddRule(CaseField.Name, MissingStart);
                return validity;
            }
            // end
            if (CaseField.EndMandatory && hasStart && !hasEnd)
            {
                validity.AddRule(CaseField.Name, MissingEnd);
                return validity;
            }
            // value
            if (hasStart && !hasValue)
            {
                validity.AddRule(CaseField.Name, MissingValue);
                return validity;
            }
        }

        // attachment validation
        if (CaseField.AttachmentType == AttachmentType.Mandatory && 
            (CaseField.Documents == null || !CaseField.Documents.Any()))
        {
            validity.AddRule(CaseField.Name, MissingAttachment);
        }

        return validity;
    }

    /// <summary>
    /// validate case field start
    /// </summary>
    /// <returns>True if the case field start is valid</returns>
    public bool ValidateStart()
    {
        // timeless
        if (CaseField.TimeType == CaseFieldTimeType.Timeless)
        {
            return true;
        }

        var hasStart = CaseField.Start.HasValue;
        // mandatory field with start and value
        if (CaseField.ValueMandatory)
        {
            // mandatory start
            return hasStart;
        }

        var hasValue = CaseField.HasValue;
        var hasEnd = CaseField.End.HasValue;
        // mandatory field with start and value
        if (CaseField.ValueMandatory)
        {
            if (!hasStart)
            {
                return false;
            }
        }
        else
        {
            // optional field value
            if (!hasStart && (hasEnd || hasValue))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// validate case field end
    /// </summary>
    /// <returns>True if the case field end is valid</returns>
    public bool ValidateEnd()
    {
        // timeless
        if (CaseField.TimeType == CaseFieldTimeType.Timeless)
        {
            return true;
        }

        // all period time types
        var hasStart = CaseField.Start.HasValue;
        var hasEnd = CaseField.End.HasValue;

        // mandatory field with start and value
        if (CaseField.ValueMandatory)
        {
            if (CaseField.EndMandatory && !hasEnd)
            {
                return false;
            }
        }
        else
        {
            // optional field value
            if (CaseField.EndMandatory && hasStart && !hasEnd)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// validate case field value
    /// </summary>
    /// <returns>True if the case field value is valid</returns>
    public bool ValidateValue()
    {
        var hasValue = CaseField.HasValue;

        // timeless
        if (CaseField.TimeType == CaseFieldTimeType.Timeless)
        {
            if (CaseField.ValueMandatory && !hasValue)
            {
                return false;
            }
            return true;
        }

        var hasStart = CaseField.Start.HasValue;
        // mandatory field with start and value
        if (CaseField.ValueMandatory)
        {
            if (!hasValue)
            {
                return false;
            }
        }
        else if (hasStart && !hasValue)
        {
            // optional field value
            return false;
        }

        return true;
    }

    /// <summary>
    /// validate case field attachment
    /// </summary>
    /// <returns>True if the case field attachment is valid</returns>
    public bool ValidateAttachment()
    {
        return CaseField.AttachmentType != AttachmentType.Mandatory ||
               (CaseField.Documents != null && CaseField.Documents.Any());
    }
}