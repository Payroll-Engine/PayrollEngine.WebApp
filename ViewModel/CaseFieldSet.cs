using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using PayrollEngine.Client.Model;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.QueryExpression;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

public class CaseFieldSet : Client.Model.CaseFieldSet, IViewModel, IKeyEquatable<CaseFieldSet>, IFieldObject
{
    private readonly bool initialized;

    // Json ignore required to suppress recursive render errors
    [JsonIgnore]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public object Input { get; set; }

    [JsonIgnore]
    private ICaseValueProvider CaseValueProvider { get; }

    [JsonIgnore]
    private CultureInfo TenantCulture { get; }

    [JsonIgnore]
    public IValueFormatter ValueFormatter { get; }

    public CaseFieldSet(Client.Model.CaseFieldSet copySource, ICaseValueProvider caseValueProvider,
        IValueFormatter valueFormatter, CultureInfo tenantCulture, Localizer localizer) :
        base(copySource)
    {
        CaseValueProvider = caseValueProvider ?? throw new ArgumentNullException(nameof(caseValueProvider));
        TenantCulture = tenantCulture ?? throw new ArgumentNullException(nameof(tenantCulture));
        ValueFormatter = valueFormatter ?? throw new ArgumentNullException(nameof(valueFormatter));

        Validator = new(this, localizer);

        // view
        DisplayName = string.IsNullOrWhiteSpace(copySource.DisplayName) ? copySource.Name : copySource.DisplayName;

        // view
        // do not set the bound fields
        // a null value also initializes zero value on the numeric edits
        if (copySource.Start.HasValue)
        {
            Start = copySource.Start;
        }
        else if (!string.IsNullOrWhiteSpace(DefaultStart))
        {
            Start = ParseDateExpression(DefaultStart);
        }

        if (copySource.End.HasValue)
        {
            End = copySource.End;
        }
        else if (!string.IsNullOrWhiteSpace(DefaultEnd))
        {
            End = ParseDateExpression(DefaultEnd);
        }

        if (copySource.Value != null)
        {
            Value = copySource.Value;
        }
        else if (!string.IsNullOrWhiteSpace(DefaultValue))
        {
            Value = ParseValueExpression(DefaultValue);
        }

        CancellationDate = copySource.CancellationDate;
        Tags = copySource.Tags;

        // initialize document handler
        Documents.Added += DocumentsHandlerAsync;
        Documents.Removed += DocumentsHandlerAsync;

        // set attachment type
        AttachmentType = Attributes.GetAttachment(TenantCulture) ?? AttachmentType.None;

        // status
        UpdateValidation();
        initialized = true;
    }

    public CaseFieldValidator Validator { get; }

    public bool IsValidValue() => Validator.ValidateValue();

    public string GetLocalizedName(CultureInfo culture) =>
        culture.Name.GetLocalization(NameLocalizations, Name);

    public string GetLocalizedDescription(CultureInfo culture) =>
        culture.Name.GetLocalization(DescriptionLocalizations, Description);

    #region Case Value

    /// <summary>
    /// The case field description markup
    /// </summary>
    public MarkupString GetDescriptionMarkup() =>
        new(Description);

    private bool HasStart { get; set; }

    private DateTime? start;
    public new DateTime? Start
    {
        get => start;
        set
        {
            if (value != start)
            {
                // limit start to end
                if (value.HasValue && end.HasValue && value.Value > end.Value)
                {
                    value = end.Value;
                }
            }
            start = value;
            HasStart = start.HasValue;
            OnFieldChangedAsync().Wait();
        }
    }

    public bool StartAvailable() =>
        TimeType.HasStart();

    public bool StartMissing()
    {
        if (TimeType == CaseFieldTimeType.Timeless || HasStart)
        {
            return false;
        }

        if (ValueMandatory)
        {
            // missing mandatory value
            return true;
        }

        // incomplete non-mandatory
        return HasValue;
    }

    private bool HasEnd { get; set; }

    private DateTime? end;
    public new DateTime? End
    {
        get => end;
        set
        {
            if (value != end)
            {
                // limit end to start
                if (value.HasValue && start.HasValue && value.Value < start.Value)
                {
                    value = start.Value;
                }
                end = value;
                HasEnd = end.HasValue;
                OnFieldChangedAsync().Wait();
            }
        }
    }

    public bool EndAvailable() =>
        TimeType.HasEnd();

    public bool OpenEnd()
    {
        if (TimeType == CaseFieldTimeType.Timeless)
        {
            return false;
        }

        // no end present -> open
        return !HasEnd;
    }

    public MarkupString GetValueMarkup()
    {
        // string
        if (ValueType == ValueType.String)
        {
            // masked text
            var mask = Attributes.GetValueMask(TenantCulture);
            if (!string.IsNullOrWhiteSpace(mask))
            {
                MaskedTextProvider maskProvider = new(mask);
                maskProvider.Set(ValueAsString);
                return new(maskProvider.ToDisplayString());
            }

            // multi line text
            var lineCount = Attributes.GetLineCount(TenantCulture);
            if (lineCount > 1)
            {
                return new(ValueFormatter.ToString(Value, ValueType, TenantCulture).Replace("\n", "<br />"));
            }

            // lookup display text
            var lookup = LookupValues?.FirstOrDefault(x => Equals(x.Value, ValueAsString));
            if (lookup != null)
            {
                return new(lookup.Text);
            }
        }

        // resource
        if (ValueType == ValueType.WebResource)
        {
            var address = Value;
            if (string.IsNullOrWhiteSpace(address))
            {
                return new();
            }
            return new($"<a href=\"{Value}\" target=\"_blank\">{Value}</a>");
        }

        // money
        if (ValueType == ValueType.Money)
        {
            var valueAsDecimal = ValueAsDecimal;
            if (valueAsDecimal.HasValue)
            {
                return new(ValueFormatter.ToString(Value, ValueType, TenantCulture));
            }
        }

        // other values
        return new(ValueFormatter.ToString(Value, ValueType, TenantCulture));
    }

    /// <summary>Gets or sets the has value</summary>
    [JsonIgnore]
    public new bool HasValue { get; private set; }

    public bool ValueTypeAvailable() =>
        ValueType != ValueType.None;

    public bool ValueMissing()
    {
        if (ValueType == ValueType.None ||
            TimeType == CaseFieldTimeType.Timeless ||
            HasValue)
        {
            return false;
        }

        // missing mandatory value
        if (ValueMandatory)
        {
            return true;
        }

        // incomplete non-mandatory
        return HasStart;
    }

    public AsyncEvent<CaseFieldSet> FieldChanged { get; set; }

    /// <summary>
    /// Updates the edit status
    /// </summary>
    /// <returns><c>true</c> if the edit status has been changed</returns>
    private async System.Threading.Tasks.Task OnFieldChangedAsync()
    {
        if (initialized)
        {
            UpdateValidation();
            // event
            await (FieldChanged?.InvokeAsync(this, this) ?? System.Threading.Tasks.Task.CompletedTask);
        }
    }

    #endregion

    #region Variant Value

    /// <summary>
    /// The case value (JSON format)
    /// </summary>
    public new string Value
    {
        get
        {
            if (ValueType.IsString())
            {
                return ValueAsString;
            }
            if (ValueType.IsDateTime())
            {
                return ValueConvert.ToJson(ValueAsDateTime);
            }
            if (ValueType.IsInteger())
            {
                return ValueConvert.ToJson(ValueAsInteger);
            }
            if (ValueType.IsDecimal())
            {
                return ValueConvert.ToJson(ValueAsDecimal);
            }
            if (ValueType.IsBoolean())
            {
                return ValueConvert.ToJson(ValueAsBoolean);
            }
            return null;
        }
        set
        {
            if (ValueType.IsString())
            {
                ValueAsString = ValueConvert.ToString(value, TenantCulture);
            }
            if (ValueType.IsDateTime())
            {
                ValueAsDateTime = Date.Parse(value, TenantCulture) ?? ValueConvert.ToDateTime(value, TenantCulture);
            }
            if (ValueType.IsInteger())
            {
                ValueAsInteger = ValueConvert.ToInteger(value, TenantCulture);
            }
            if (ValueType.IsDecimal())
            {
                ValueAsDecimal = ValueConvert.ToDecimal(value, TenantCulture);
            }
            if (ValueType.IsBoolean())
            {
                ValueAsBoolean = ValueConvert.ToBoolean(value, TenantCulture);
            }
        }
    }

    private string stringValue;
    /// <inheritdoc />
    [JsonIgnore]
    public string ValueAsString
    {
        get => stringValue;
        set
        {
            if (value != stringValue)
            {
                stringValue = value;
                HasValue = value != null && !string.Empty.Equals(value);
                OnFieldChangedAsync().Wait();
            }
        }
    }

    private DateTime? dateTimeValue;
    /// <inheritdoc />
    [JsonIgnore]
    public DateTime? ValueAsDateTime
    {
        get => dateTimeValue;
        set
        {
            if (value != dateTimeValue)
            {
                dateTimeValue = value;
                HasValue = dateTimeValue.HasValue;
                OnFieldChangedAsync().Wait();
            }
        }
    }

    private int? integerValue;
    /// <inheritdoc />
    [JsonIgnore]
    public int? ValueAsInteger
    {
        get => integerValue;
        set
        {
            if (value != integerValue)
            {
                integerValue = value;
                HasValue = integerValue.HasValue;
                OnFieldChangedAsync().Wait();
            }
        }
    }

    private decimal? decimalValue;
    /// <inheritdoc />
    [JsonIgnore]
    public decimal? ValueAsDecimal
    {
        get => decimalValue;
        set
        {
            if (value != decimalValue)
            {
                decimalValue = value;
                HasValue = decimalValue.HasValue;
                OnFieldChangedAsync().Wait();
            }
        }
    }

    /// <inheritdoc />
    [JsonIgnore]
    public decimal? ValueAsPercent
    {
        get => ValueAsDecimal != null ? ValueAsDecimal * 100 : 0;
        set => ValueAsDecimal = value / 100;
    }

    private bool? booleanValue;
    /// <inheritdoc />
    [JsonIgnore]
    public bool? ValueAsBoolean
    {
        get => booleanValue;
        set
        {
            if (value != booleanValue)
            {
                booleanValue = value;
                HasValue = booleanValue.HasValue;
                OnFieldChangedAsync().Wait();
            }
        }
    }

    #endregion

    #region Documents

    public AttachmentType AttachmentType { get; }

    public ObservedHashSet<CaseDocument> Documents { get; } = new();

    private async System.Threading.Tasks.Task DocumentsHandlerAsync(object sender, CaseDocument document)
    {
        UpdateValidation();
        await OnFieldChangedAsync();
    }

    #region Lookup

    public List<LookupObject> LookupValues { get; } = new();

    #endregion

    #endregion

    #region History

    private List<CaseValueSetup> historyValues;
    public List<CaseValueSetup> HistoryValues =>
        historyValues ??= System.Threading.Tasks.Task.Run(LoadHistoryValues).Result;

    private async System.Threading.Tasks.Task<List<CaseValueSetup>> LoadHistoryValues()
    {
        // query
        var query = new CaseValueQuery();
        AppendFilter(query, nameof(CaseValue.CaseFieldName), Name);
        AppendFilter(query, nameof(CaseValue.CaseSlot), CaseSlot);
        var values = await CaseValueProvider.GetCaseValuesAsync(query);
        return values.Select(x => new CaseValueSetup(x)).ToList();
    }

    /// <summary>
    /// Append filter to existing query dynamically
    /// </summary>
    /// <param name="query">The query to be updated with new filter</param>
    /// <param name="field">Filter field name</param>
    /// <param name="value">Filter value</param>
    /// <returns></returns>
    private static void AppendFilter(Query query, string field, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            // input string empty
            return;
        }

        var filter = new Equals(field, value);
        if (string.IsNullOrWhiteSpace(query.Filter))
        {
            query.Filter = filter;
        }
        else
        {
            query.Filter += $" {filter}'";
        }
    }

    #endregion

    #region Validation

    public CaseObjectValidity Validity { get; private set; } = new();

    /// <summary>
    /// Updates the validation status
    /// </summary>
    private void UpdateValidation() =>
        Validity = Validator.Validate();


    #endregion

    private string ParseValueExpression(string expression)
    {
        if (!ValueType.IsDateTime())
        {
            return expression;
        }

        var date = ParseDateExpression(expression);
        if (date.HasValue)
        {
            expression = ValueConvert.ToJson(date.Value);
        }
        return expression;
    }

    private DateTime? ParseDateExpression(string expression) =>
        Date.Parse(expression, TenantCulture);

    public bool EqualKey(CaseFieldSet compare) =>
        base.EqualKey(compare);

    public override string ToString()
    {
        var buffer = new StringBuilder();
        buffer.Append(Name);
        if (!string.IsNullOrWhiteSpace(Value))
        {
            buffer.Append($"={Value}");
        }
        if (Start.HasValue)
        {
            buffer.Append($", start={Start.Value}");
        }
        if (End.HasValue)
        {
            buffer.Append($", end={End.Value}");
        }
        if (CancellationDate.HasValue)
        {
            buffer.Append(" [Cancel]");
        }
        buffer.Append($" [{Id}]");
        return buffer.ToString();
    }

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(CaseFieldSet compare) =>
        base.Equals(compare);

    /// <summary>Compare two objects</summary>
    /// <param name="compare">The object to compare with this</param>
    /// <returns>True for objects with the same data</returns>
    public bool Equals(IViewModel compare) =>
        Equals(compare as CaseFieldSet);

    //public void Dispose()
    //{
    //    LookupFields?.Dispose();
    //    Lookups?.Dispose();
    //}
}