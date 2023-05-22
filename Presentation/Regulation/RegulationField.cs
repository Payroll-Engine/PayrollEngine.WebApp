using System;

namespace PayrollEngine.WebApp.Presentation.Regulation;

/// <summary>
/// Regulation edit field
/// </summary>
public class RegulationField
{
    /// <summary>
    /// THe property name
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// The field label
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    /// Indicates a key field
    /// </summary>
    public bool KeyField { get; set; }

    /// <summary>
    /// Indicates a read only field
    /// </summary>
    public bool ReadOnly { get; set; }

    /// <summary>
    /// Base value is fixed
    /// </summary>
    public bool FixedBaseValue { get; set; }

    /// <summary>
    /// Indicates a required field
    /// </summary>
    public bool Required { get; set; }

    /// <summary>
    /// The required error message
    /// </summary>
    public string RequiredError { get; set; }

    /// <summary>
    /// Custom field group
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// The help message
    /// </summary>
    private string help;
    public string Help
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(help))
            {
                return help;
            }
            return Expression && !IsAction ? "C# Expression" : null;
        }
        set => help = value;
    }

    /// <summary>
    /// Indicates an expression field
    /// </summary>
    public bool Expression { get; set; }

    /// <summary>
    /// The function action type
    /// </summary>
    public FunctionType Action { get; set; }

    /// <summary>
    /// True for action field
    /// </summary>
    public bool IsAction => Action != 0;

    /// <summary>
    /// Get the action field name
    /// </summary>
    public string GetActionFieldName()
    {
        if (string.IsNullOrWhiteSpace(PropertyName))
        {
            return null;
        }

        if (PropertyName.EndsWith("Actions"))
        {
            return PropertyName;
        }
        if (PropertyName.EndsWith("Expression"))
        {
            return PropertyName.RemoveFromEnd("Expression").EnsureEnd("Actions");
        }

        return null;
    }

    /// <summary>
    /// The number of edit lines (default is 1)
    /// </summary>
    public int Lines { get; set; } = 1;

    /// <summary>
    /// The value maximum length (default is 524288)
    /// </summary>
    public int MaxLength { get; set; } = 524288;

    /// <summary>
    /// The value format
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// The UI component type
    /// </summary>
    public Type ComponentType { get; set; }
    public bool IsAttributeField =>
        string.Equals(PropertyName, nameof(IAttributeObject.Attributes));

    /// <summary>
    /// True for base values
    /// </summary>
    public bool HasBaseValues =>
        !Required &&
        !FixedBaseValue &&
        !ReadOnly;

    /// <summary>
    /// Default constructor
    /// </summary>
    public RegulationField()
    {
    }

    /// <summary>
    /// Value constructor
    /// </summary>
    /// <param name="propertyName">The property name</param>
    /// <param name="componentType">The component type</param>
    public RegulationField(string propertyName, Type componentType)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }
        if (componentType != null && componentType.GetInterface(nameof(IRegulationInput)) == null)
        {
            throw new ArgumentException($"Component type must be implement interface {nameof(IRegulationInput)}", nameof(componentType));
        }

        PropertyName = propertyName;
        ComponentType = componentType;
    }

    /// <inheritdoc />
    public override string ToString() => PropertyName;
}