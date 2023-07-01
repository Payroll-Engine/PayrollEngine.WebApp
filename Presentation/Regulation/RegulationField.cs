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
    public string PropertyName { get; }

    /// <summary>
    /// The field label
    /// </summary>
    public string Label { get; init; }

    /// <summary>
    /// The field action label
    /// </summary>
    public string ActionLabel { get; init; }

    /// <summary>
    /// Indicates a key field
    /// </summary>
    public bool KeyField { get; init; }

    /// <summary>
    /// Indicates a read only field
    /// </summary>
    public bool ReadOnly { get; init; }

    /// <summary>
    /// Base value is fixed
    /// </summary>
    public bool FixedBaseValue { get; init; }

    /// <summary>
    /// Indicates a required field
    /// </summary>
    public bool Required { get; init; }

    /// <summary>
    /// The required error message
    /// </summary>
    public string RequiredError { get; init; }

    /// <summary>
    /// Custom field group
    /// </summary>
    public string Group { get; init; }

    /// <summary>
    /// The help message
    /// </summary>
    private readonly string help;
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
        init => help = value;
    }

    /// <summary>
    /// Indicates an expression field
    /// </summary>
    public bool Expression { get; init; }

    /// <summary>
    /// The function action type
    /// </summary>
    public FunctionType Action { get; init; }

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

        // action
        if (PropertyName.EndsWith("Actions"))
        {
            return PropertyName;
        }

        // expression
        if (PropertyName.EndsWith("Expression"))
        {
            return PropertyName.RemoveFromEnd("Expression").EnsureEnd("Actions");
        }

        return null;
    }

    /// <summary>
    /// The number of edit lines (default is 1)
    /// </summary>
    public int Lines { get; init; } = 1;

    /// <summary>
    /// The value maximum length (default is 524288)
    /// </summary>
    public int MaxLength { get; init; } = 524288;

    /// <summary>
    /// The value format
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string Format { get; set; }

    /// <summary>
    /// The UI component type
    /// </summary>
    public Type ComponentType { get; }
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
            throw new ArgumentException(nameof(componentType));
        }

        PropertyName = propertyName;
        ComponentType = componentType;
    }

    /// <inheritdoc />
    public override string ToString() => PropertyName;
}