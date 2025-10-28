namespace Selmir.MudGridify.Models;

/// <summary>
/// Represents a property that can be filtered
/// </summary>
public class FilterableProperty
{
    /// <summary>
    /// The property name (must match the actual property name in your model)
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown to the user
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The data type of the property
    /// </summary>
    public FilterPropertyType PropertyType { get; set; }

    /// <summary>
    /// For boolean types, optional custom labels (default: "True"/"False")
    /// </summary>
    public string? TrueLabel { get; set; }
    public string? FalseLabel { get; set; }

    /// <summary>
    /// Constructor for convenience
    /// </summary>
    public FilterableProperty() { }

    /// <summary>
    /// Constructor with required parameters
    /// </summary>
    public FilterableProperty(string propertyName, string displayName, FilterPropertyType propertyType)
    {
        PropertyName = propertyName;
        DisplayName = displayName;
        PropertyType = propertyType;
    }
}
