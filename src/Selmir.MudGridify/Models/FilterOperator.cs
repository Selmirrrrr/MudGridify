namespace Selmir.MudGridify.Models;

/// <summary>
/// Gridify filter operators
/// </summary>
public enum FilterOperator
{
    // Universal operators
    Equals,           // =
    NotEquals,        // !=

    // Numeric and date operators
    GreaterThan,      // >
    LessThan,         // <
    GreaterOrEqual,   // >=
    LessOrEqual,      // <=

    // String operators
    Contains,         // =*
    NotContains,      // !*
    StartsWith,       // ^
    NotStartsWith,    // !^
    EndsWith,         // $
    NotEndsWith       // !$
}

/// <summary>
/// Extension methods for FilterOperator
/// </summary>
public static class FilterOperatorExtensions
{
    /// <summary>
    /// Converts FilterOperator to Gridify operator string
    /// </summary>
    public static string ToGridifyOperator(this FilterOperator op)
    {
        return op switch
        {
            FilterOperator.Equals => "=",
            FilterOperator.NotEquals => "!=",
            FilterOperator.GreaterThan => ">",
            FilterOperator.LessThan => "<",
            FilterOperator.GreaterOrEqual => ">=",
            FilterOperator.LessOrEqual => "<=",
            FilterOperator.Contains => "=*",
            FilterOperator.NotContains => "!*",
            FilterOperator.StartsWith => "^",
            FilterOperator.NotStartsWith => "!^",
            FilterOperator.EndsWith => "$",
            FilterOperator.NotEndsWith => "!$",
            _ => "="
        };
    }

    /// <summary>
    /// Gets display name for the operator (localized)
    /// </summary>
    public static string GetDisplayName(this FilterOperator op, Microsoft.Extensions.Localization.IStringLocalizer<Resources.Localization>? localizer = null)
    {
        if (localizer == null)
        {
            // Fallback to English if no localizer provided
            return op switch
            {
                FilterOperator.Equals => "Equals",
                FilterOperator.NotEquals => "Not Equals",
                FilterOperator.GreaterThan => "Greater Than",
                FilterOperator.LessThan => "Less Than",
                FilterOperator.GreaterOrEqual => "Greater or Equal",
                FilterOperator.LessOrEqual => "Less or Equal",
                FilterOperator.Contains => "Contains",
                FilterOperator.NotContains => "Not Contains",
                FilterOperator.StartsWith => "Starts With",
                FilterOperator.NotStartsWith => "Not Starts With",
                FilterOperator.EndsWith => "Ends With",
                FilterOperator.NotEndsWith => "Not Ends With",
                _ => op.ToString()
            };
        }

        return op switch
        {
            FilterOperator.Equals => localizer["Operator_Equals"],
            FilterOperator.NotEquals => localizer["Operator_NotEquals"],
            FilterOperator.GreaterThan => localizer["Operator_GreaterThan"],
            FilterOperator.LessThan => localizer["Operator_LessThan"],
            FilterOperator.GreaterOrEqual => localizer["Operator_GreaterOrEqual"],
            FilterOperator.LessOrEqual => localizer["Operator_LessOrEqual"],
            FilterOperator.Contains => localizer["Operator_Contains"],
            FilterOperator.NotContains => localizer["Operator_NotContains"],
            FilterOperator.StartsWith => localizer["Operator_StartsWith"],
            FilterOperator.NotStartsWith => localizer["Operator_NotStartsWith"],
            FilterOperator.EndsWith => localizer["Operator_EndsWith"],
            FilterOperator.NotEndsWith => localizer["Operator_NotEndsWith"],
            _ => op.ToString()
        };
    }

    /// <summary>
    /// Gets available operators for a property type
    /// </summary>
    public static FilterOperator[] GetOperatorsForType(FilterPropertyType type)
    {
        return type switch
        {
            FilterPropertyType.String => new[]
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.Contains,
                FilterOperator.NotContains,
                FilterOperator.StartsWith,
                FilterOperator.NotStartsWith,
                FilterOperator.EndsWith,
                FilterOperator.NotEndsWith
            },
            FilterPropertyType.Number or FilterPropertyType.Date or FilterPropertyType.DateTime => new[]
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals,
                FilterOperator.GreaterThan,
                FilterOperator.LessThan,
                FilterOperator.GreaterOrEqual,
                FilterOperator.LessOrEqual
            },
            FilterPropertyType.Boolean => new[]
            {
                FilterOperator.Equals,
                FilterOperator.NotEquals
            },
            _ => new[] { FilterOperator.Equals, FilterOperator.NotEquals }
        };
    }
}
