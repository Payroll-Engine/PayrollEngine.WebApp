using System;
using System.Globalization;
using System.Linq;
using System.Text;
using PayrollEngine.WebApp.Shared;

namespace PayrollEngine.WebApp.ViewModel;

public class CaseSet : Case, IDisposable
{
    private CultureInfo TenantCulture { get; }

    public CaseSet(Client.Model.CaseSet copySource, ICaseValueProvider caseValueProvider,
        IValueFormatter valueFormatter, CultureInfo tenantCulture, Localizer localizer) :
        base(copySource)
    {
        // culture
        TenantCulture = tenantCulture ?? throw new ArgumentNullException(nameof(tenantCulture));

        // fields
        if (copySource.Fields != null)
        {
            var sourceFields = new ObservedHashSet<CaseFieldSet>();

            foreach (var field in copySource.Fields)
            {
                // ignore inactive case fields
                if (field.Status == ObjectStatus.Active)
                {
                    sourceFields.AddAsync(new(field, caseValueProvider, valueFormatter, tenantCulture, localizer)).Wait();
                }
            }
            Fields = sourceFields;
        }

        // related cases
        if (copySource.RelatedCases != null)
        {
            var sourceRelatedCases = new ObservedHashSet<CaseSet>();
            foreach (var relatedCase in copySource.RelatedCases)
            {
                // ignore inactive related cases
                if (relatedCase.Status == ObjectStatus.Active)
                {
                    sourceRelatedCases.AddAsync(new(relatedCase, caseValueProvider, valueFormatter, tenantCulture, localizer)).Wait();
                }
            }

            RelatedCases = sourceRelatedCases;
        }

        UpdateValidationAsync().Wait();
    }

    #region Fields

    /// <summary>
    /// The case fields
    /// </summary>
    private readonly ObservedHashSet<CaseFieldSet> fields;
    public ObservedHashSet<CaseFieldSet> Fields
    {
        get => fields;
        private init
        {
            DisconnectFields();
            fields = value;
            ConnectFields();
        }
    }

    public bool HasAnyField =>
        Fields != null && Fields.Any();

    public AsyncEvent<CaseSet> FieldChanged { get; set; }

    /// <summary>
    /// Updates the edit status
    /// </summary>
    /// <returns><c>true</c> if the edit status has been changed</returns>
    private async System.Threading.Tasks.Task OnFieldChangedAsync(CaseSet source)
    {
        await UpdateValidationAsync();
        // event
        await (FieldChanged?.InvokeAsync(this, source) ?? System.Threading.Tasks.Task.CompletedTask);
    }

    private void ConnectFields()
    {
        if (Fields != null)
        {
            foreach (var caseField in Fields)
            {
                caseField.FieldChanged += FieldChangedHandlerAsync;
            }
            Fields.Added += FieldAdded;
            Fields.Removed += FieldRemoved;
        }
    }

    private void DisconnectFields()
    {
        if (Fields != null)
        {
            foreach (var caseField in Fields)
            {
                caseField.FieldChanged -= FieldChangedHandlerAsync;
            }
            Fields.Added -= FieldAdded;
            Fields.Removed -= FieldRemoved;
        }
    }

    private async System.Threading.Tasks.Task FieldAdded(object sender, CaseFieldSet caseField)
    {
        caseField.FieldChanged += FieldChangedHandlerAsync;
        await UpdateValidationAsync();
    }

    private async System.Threading.Tasks.Task FieldRemoved(object sender, CaseFieldSet caseField)
    {
        caseField.FieldChanged -= FieldChangedHandlerAsync;
        await UpdateValidationAsync();
    }

    private async System.Threading.Tasks.Task FieldChangedHandlerAsync(object sender, CaseFieldSet caseField) =>
        await OnFieldChangedAsync(this);

    #endregion

    #region Related Cases

    /// <summary>
    /// Gets or sets the related cases
    /// </summary>
    private ObservedHashSet<CaseSet> relatedCases;
    public ObservedHashSet<CaseSet> RelatedCases
    {
        get => relatedCases;
        set
        {
            DisconnectRelatedCases();
            relatedCases = value;
            ConnectRelatedCases();
        }
    }

    /// <summary>
    /// Set related cases or return visible related cases (without hidden attribute)
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public ObservedHashSet<CaseSet> VisibleRelatedCases
    {
        get
        {
            var visibleRelatedCases = new ObservedHashSet<CaseSet>();
            if (relatedCases == null)
            {
                return visibleRelatedCases;
            }
            foreach (var relatedCase in relatedCases)
            {
                if (!relatedCase.Fields.All(f => f.Attributes?.GetHidden(TenantCulture) ?? false))
                {
                    visibleRelatedCases.AddAsync(relatedCase).Wait();
                }
            }

            return visibleRelatedCases;
        }
        set
        {
            DisconnectRelatedCases();
            relatedCases = value;
            ConnectRelatedCases();
        }
    }

    public bool HasAnyRelatedCase =>
        RelatedCases != null && RelatedCases.Any();

    private void ConnectRelatedCases()
    {
        if (RelatedCases != null)
        {
            foreach (var relatedCase in RelatedCases)
            {
                relatedCase.FieldChanged += RelatedCaseFieldChangedHandlerAsync;
            }
            RelatedCases.Added += RelatedCaseAddedHandler;
            RelatedCases.Removed += RelatedCaseRemovedHandlerAsync;
        }
    }

    private void DisconnectRelatedCases()
    {
        if (RelatedCases != null)
        {
            foreach (var relatedCase in RelatedCases)
            {
                relatedCase.FieldChanged -= RelatedCaseFieldChangedHandlerAsync;
            }
            RelatedCases.Added -= RelatedCaseAddedHandler;
            RelatedCases.Removed -= RelatedCaseRemovedHandlerAsync;
        }
    }

    private async System.Threading.Tasks.Task RelatedCaseAddedHandler(object sender, CaseSet caseSet)
    {
        caseSet.FieldChanged += RelatedCaseFieldChangedHandlerAsync;
        await UpdateValidationAsync();
    }

    private async System.Threading.Tasks.Task RelatedCaseRemovedHandlerAsync(object sender, CaseSet caseSet)
    {
        caseSet.FieldChanged -= RelatedCaseFieldChangedHandlerAsync;
        await UpdateValidationAsync();
    }

    private async System.Threading.Tasks.Task RelatedCaseFieldChangedHandlerAsync(object sender, CaseSet relatedCaseSet)
    {
        await UpdateValidationAsync();
        await OnFieldChangedAsync(relatedCaseSet);
    }

    #endregion

    #region View

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="CaseSet"/> is selected
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool Selected { get; set; }

    /// <summary>
    /// Required for the tree view field settings binding Expanded
    /// </summary>
    public bool IsExpanded { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the case fields are valid
    /// </summary>
    public CaseObjectValidity Validity { get; private set; }

    /// <summary>
    /// Updates the validation status
    /// </summary>
    private async System.Threading.Tasks.Task UpdateValidationAsync()
    {
        var newValidity = await GetValidStateAsync(this);
        if (!newValidity.Equals(Validity))
        {
            // status update
            Validity = newValidity;
        }
    }

    private async System.Threading.Tasks.Task<CaseObjectValidity> GetValidStateAsync(CaseSet caseSet, CaseObjectValidity validity = null)
    {
        // initialize validity object if called for first time
        // with default value valid=true
        validity ??= new();

        // find invalid case field
        var invalidFields = caseSet.Fields?.Where(x => !x.Validity.Valid).ToList();
        if (invalidFields != null && invalidFields.Any())
        {
            foreach (var invalidField in invalidFields)
            {
                validity.AddRules(invalidField.Validity.Rules);
            }
            return validity;
        }

        // recursive check for related cases
        if (caseSet.RelatedCases != null)
        {
            foreach (var relatedCase in caseSet.RelatedCases)
            {
                await GetValidStateAsync(relatedCase, validity);
            }
            return validity;
        }

        return validity;
    }

    #endregion

    void IDisposable.Dispose()
    {
        DisconnectFields();
        DisconnectRelatedCases();
    }

    public override string ToString()
    {
        var buffer = new StringBuilder();
        buffer.Append(Name);
        if (Fields != null && Fields.Any())
        {
            buffer.Append($", {Fields.Count} fields");
        }
        if (RelatedCases != null && RelatedCases.Any())
        {
            buffer.Append($", {RelatedCases.Count} relations");
        }
        buffer.Append($" [{Id}]");
        return buffer.ToString();
    }
}