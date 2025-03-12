using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using PayrollEngine.Client.Model;
using PayrollEngine.WebApp.Shared;
using PayrollEngine.Client.QueryExpression;

namespace PayrollEngine.WebApp.ViewModel;

/// <summary>
/// View model case filed set
/// </summary>
public class CaseFieldSet : Client.Model.CaseFieldSet, IViewModel, IKeyEquatable<CaseFieldSet>, IFieldObject
{
    private readonly bool initialized;

    /// <summary>
    /// Input value
    /// </summary>
    // Json ignore required to suppress recursive render errors
    [JsonIgnore]
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public object Input { get; set; }

    [JsonIgnore]
    private ICaseValueProvider CaseValueProvider { get; }

    [JsonIgnore]
    private CultureInfo TenantCulture { get; }

    [JsonIgnore]
    private IValueFormatter ValueFormatter { get; }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copySource">Copy source</param>
    /// <param name="caseValueProvider">Case value provider</param>
    /// <param name="valueFormatter">Value formatter</param>
    /// <param name="tenantCulture">Tenant culture</param>
    /// <param name="localizer">Localizer</param>
    /// <exception cref="ArgumentNullException"></exception>
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
        // start
        if (copySource.Start.HasValue)
        {
            Start = copySource.Start;
        }
        else if (!string.IsNullOrWhiteSpace(DefaultStart))
        {
            Start = ParseDateExpression(DefaultStart);
        }

        // end
        if (copySource.End.HasValue)
        {
            End = copySource.End;
        }
        else if (!string.IsNullOrWhiteSpace(DefaultEnd))
        {
            End = ParseDateExpression(DefaultEnd);
        }

        // value
        if (copySource.Value != null && !string.Equals(copySource.Value, DefaultValue))
        {
            Value = copySource.Value;
        }
        else if (!string.IsNullOrWhiteSpace(DefaultValue))
        {
            Value = ParseValueExpression(DefaultValue);
        }

        // misc
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

    /// <summary>
    /// Validator
    /// </summary>
    public CaseFieldValidator Validator { get; }

    /// <summary>
    /// Test for valid value
    /// </summary>
    public bool IsValidValue() => Validator.ValidateValue();

    /// <summary>
    /// Get the localized name
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedName(CultureInfo culture) =>
        culture.Name.GetLocalization(NameLocalizations, Name);

    /// <summary>
    /// Get localized description
    /// </summary>
    /// <param name="culture">Culture</param>
    public string GetLocalizedDescription(CultureInfo culture) =>
        culture.Name.GetLocalization(DescriptionLocalizations, Description);

    /// <summary>
    /// Format value
    /// </summary>
    /// <param name="culture">Culture</param>
    public string FormatValue(CultureInfo culture = null)
    {
        // priority 1: object culture
        if (!string.IsNullOrWhiteSpace(Culture))
        {
            culture = new CultureInfo(Culture);
        }
        // priority 2: parameter culture
        // priority 3: system culture
        culture ??= CultureInfo.CurrentCulture;

        return ValueFormatter.ToString(Value, ValueType, culture);
    }

    #region Case Value

    /// <summary>
    /// The case field description markup
    /// </summary>
    public MarkupString GetDescriptionMarkup() =>
        new(Description);

    private bool HasStart { get; set; }

    /// <summary>
    /// Case field start date
    /// </summary>
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
            OnFieldChanged();
        }
    }

    /// <summary>
    /// start available
    /// </summary>
    public bool StartAvailable() =>
        TimeType.HasStart();

    /// <summary>
    /// Test start is missing
    /// </summary>
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
    /// <summary>
    /// Case field end date
    /// </summary>
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
                OnFieldChanged();
            }
        }
    }

    /// <summary>
    /// Test end date is available
    /// </summary>
    public bool EndAvailable() =>
        TimeType.HasEnd();

    /// <summary>
    /// Test for open end
    /// </summary>
    public bool OpenEnd()
    {
        if (TimeType == CaseFieldTimeType.Timeless)
        {
            return false;
        }

        // no end present -> open
        return !HasEnd;
    }

    /// <summary>
    /// Get value markup
    /// </summary>
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
                return new(FormatValue(TenantCulture).Replace("\n", "<br />"));
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
            return new(HtmlTool.BuildWebLink(address));
        }

        // money
        if (ValueType == ValueType.Money)
        {
            var valueAsDecimal = ValueAsDecimal;
            if (valueAsDecimal.HasValue)
            {
                return new(FormatValue(TenantCulture));
            }
        }

        // percent
        if (ValueType == ValueType.Percent)
        {
            var valueAsPercent = ValueAsPercent;
            if (valueAsPercent.HasValue)
            {
                return new(FormatValue(TenantCulture));
            }
        }

        // other values
        return new(FormatValue(TenantCulture));
    }

    /// <summary>Gets or sets the has value</summary>
    [JsonIgnore]
    public new bool HasValue { get; private set; }

    /// <summary>
    /// Tet for available value type
    /// </summary>
    public bool ValueTypeAvailable() =>
        ValueType != ValueType.None;

    /// <summary>
    /// Test if value is missing
    /// </summary>
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

    /// <summary>
    /// Event handler on filed change
    /// </summary>
    public AsyncEvent<CaseFieldSet> FieldChanged;

    /// <summary>
    /// Updates the edit status sync
    /// </summary>
    /// <returns><c>true</c> if the edit status has been changed</returns>
    private void OnFieldChanged() =>
        System.Threading.Tasks.Task.Run(OnFieldChangedAsync);

    /// <summary>
    /// Updates the edit status sync
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
                OnFieldChanged();
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
                OnFieldChanged();
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
                OnFieldChanged();
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
                OnFieldChanged();
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
                OnFieldChanged();
            }
        }
    }

    #endregion

    #region Documents

    public AttachmentType AttachmentType { get; }

    /// <summary>
    /// Case filed documents
    /// </summary>
    public ObservedHashSet<CaseDocument> Documents { get; } = [];

    private async System.Threading.Tasks.Task DocumentsHandlerAsync(object sender, CaseDocument document)
    {
        UpdateValidation();
        await OnFieldChangedAsync();
    }

    #region Lookup

    /// <summary>
    /// Lookup values
    /// </summary>
    public List<LookupObject> LookupValues { get; } = [];

    #endregion

    #endregion

    #region History

    /// <summary>
    /// Load history values
    /// </summary>
    public async System.Threading.Tasks.Task<List<CaseValueSetup>> LoadHistoryValuesAsync()
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

    /// <summary>
    /// Get validity
    /// </summary>
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

    /// <summary>
    /// Check for equal key
    /// </summary>
    /// <param name="compare">Compare field</param>
    public bool EqualKey(CaseFieldSet compare) =>
        base.EqualKey(compare);

    /// <inheritdoc />
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
}