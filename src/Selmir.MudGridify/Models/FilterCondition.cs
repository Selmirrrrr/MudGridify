namespace Selmir.MudGridify.Models;

/// <summary>
/// Represents a single filter condition
/// </summary>
public class FilterCondition
{
    /// <summary>
    /// Unique identifier for this condition
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The property being filtered
    /// </summary>
    public FilterableProperty? Property { get; set; }

    /// <summary>
    /// The operator to apply
    /// </summary>
    public FilterOperator Operator { get; set; } = FilterOperator.Equals;

    /// <summary>
    /// The value to filter by
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Whether to apply case-insensitive matching (for strings)
    /// </summary>
    public bool CaseInsensitive { get; set; } = false;

    /// <summary>
    /// Checks if the condition is valid
    /// </summary>
    public bool IsValid()
    {
        if (Property == null || string.IsNullOrWhiteSpace(Property.PropertyName))
            return false;

        // Boolean values don't need a value (can be true/false)
        if (Property.PropertyType == FilterPropertyType.Boolean)
            return true;

        // Other types need a value
        return !string.IsNullOrWhiteSpace(Value);
    }

    /// <summary>
    /// Converts the condition to a Gridify filter string
    /// </summary>
    public string ToGridifyString()
    {
        if (!IsValid() || Property == null)
            return string.Empty;

        var propertyName = Property.PropertyName;
        var operatorStr = Operator.ToGridifyOperator();
        var value = Value ?? string.Empty;

        // Add case-insensitive flag for strings
        if (CaseInsensitive && Property.PropertyType == FilterPropertyType.String && !string.IsNullOrWhiteSpace(value))
        {
            value = $"{value}/i";
        }

        // For date/datetime, format appropriately
        if (Property.PropertyType == FilterPropertyType.Date || Property.PropertyType == FilterPropertyType.DateTime)
        {
            // Gridify accepts ISO format dates
            if (DateTime.TryParse(value, out var dateValue))
            {
                value = Property.PropertyType == FilterPropertyType.Date
                    ? dateValue.ToString("yyyy-MM-dd")
                    : dateValue.ToString("yyyy-MM-ddTHH:mm:ss");
            }
        }

        return $"{propertyName}{operatorStr}{value}";
    }
}
