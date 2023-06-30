// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
namespace PayrollEngine.WebApp;

// ReSharper disable once ClassNeverInstantiated.Global
public class GridColumnConfiguration
{
    /// <summary>
    /// The property of the JSON to be shown in cell
    /// </summary>
    public string Attribute { get; set; }

    /// <summary>
    /// Header Text to be displayed in column
    /// </summary>
    public string Header { get; set; }

    /// <summary>
    /// The value type of the JSON value
    /// </summary>
    public ValueType ValueType { get; set; }

    /// <summary>
    /// The column format
    /// </summary>
    public string Format { get; set; }

    /// <summary>
    /// The column width, default is 10%
    /// </summary>
    public string Width { get; set; } = "10%";

    /// <summary>
    /// Allow sorting, default is true
    /// </summary>
    public bool AllowSorting { get; set; } = true;

    /// <summary>
    /// Allow grouping, default is true
    /// </summary>
    public bool AllowGrouping { get; set; } = true;

    /// <summary>
    /// Allow filtering, default is true
    /// </summary>
    public bool AllowFiltering { get; set; } = true;

    // test for valid column configuration
    public bool IsValid =>
        !string.IsNullOrWhiteSpace(Attribute);

    // column name
    public string ColumnName =>
        IsValid ? ToPropertyName(Attribute) : null;

    // column header
    public string ColumnHeader
    {
        get
        {
            if (!IsValid)
            {
                return null;
            }
            return !string.IsNullOrWhiteSpace(Header) ? Header : Attribute.FirstCharacterToUpper();
        }
    }

    private static string ToPropertyName(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return null;
        }
        return propertyName
            .Replace('.', '_')
            .Replace(':', '_')
            .Replace('$', '_')
            .Replace('?', '_');
    }

}