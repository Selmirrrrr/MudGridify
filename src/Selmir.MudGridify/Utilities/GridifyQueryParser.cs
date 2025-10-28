using System.Globalization;
using System.Text.RegularExpressions;
using Selmir.MudGridify.Models;

namespace Selmir.MudGridify.Utilities;

/// <summary>
/// Utility class for parsing Gridify query strings back into FilterCondition objects.
/// Used for URL persistence and state restoration.
/// </summary>
public static class GridifyQueryParser
{
    /// <summary>
    /// Parses a Gridify query string into a list of FilterCondition objects.
    /// </summary>
    /// <param name="gridifyQuery">The Gridify query string (e.g., "FirstName=John,Age>30" or "Dept=Sales|Dept=Marketing")</param>
    /// <param name="filterableProperties">The list of filterable properties to match against</param>
    /// <returns>A tuple containing the parsed conditions and whether OR operator is used</returns>
    public static (List<FilterCondition> conditions, bool isOrOperator) Parse(
        string? gridifyQuery,
        List<FilterableProperty> filterableProperties)
    {
        var conditions = new List<FilterCondition>();

        if (string.IsNullOrWhiteSpace(gridifyQuery))
        {
            return (conditions, false);
        }

        // Detect logical operator: pipe = OR, comma = AND
        var isOrOperator = gridifyQuery.Contains('|');
        var separator = isOrOperator ? '|' : ',';

        // Split into individual condition segments
        var segments = gridifyQuery.Split(separator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var segment in segments)
        {
            var condition = ParseCondition(segment.Trim(), filterableProperties);
            if (condition != null)
            {
                conditions.Add(condition);
            }
        }

        return (conditions, isOrOperator);
    }

    private static FilterCondition? ParseCondition(string segment, List<FilterableProperty> filterableProperties)
    {
        if (string.IsNullOrWhiteSpace(segment))
        {
            return null;
        }

        // Check for case-insensitive flag (/i suffix)
        var caseInsensitive = false;
        if (segment.EndsWith("/i", StringComparison.Ordinal))
        {
            caseInsensitive = true;
            segment = segment[..^2]; // Remove /i suffix
        }

        // Parse the condition using regex to handle all operators
        // Pattern: PropertyName + Operator + Value
        // Operators must be checked in order of longest first to avoid conflicts (e.g., != before !, >= before >)
        var match = Regex.Match(segment, @"^([a-zA-Z_][a-zA-Z0-9_]*)(!=|!\\*|!\\^|!\\$|>=|<=|=\\*|>|<|=|\\^|\\$)(.*)$");

        if (!match.Success)
        {
            return null;
        }

        var propertyName = match.Groups[1].Value;
        var operatorString = match.Groups[2].Value;
        var value = match.Groups[3].Value;

        // Find the matching FilterableProperty
        var property = filterableProperties.FirstOrDefault(p => p.PropertyName.Equals(propertyName, StringComparison.OrdinalIgnoreCase));

        if (property == null)
        {
            return null;
        }

        // Map operator string to FilterOperator enum
        var filterOperator = MapOperator(operatorString);
        if (filterOperator == null)
        {
            return null;
        }

        // Parse value based on property type
        var parsedValue = ParseValue(value, property.PropertyType);
        if (parsedValue == null && property.PropertyType != FilterPropertyType.Boolean)
        {
            return null;
        }

        return new FilterCondition
        {
            Property = property,
            Operator = filterOperator.Value,
            Value = parsedValue,
            CaseInsensitive = caseInsensitive
        };
    }

    private static FilterOperator? MapOperator(string operatorString)
    {
        return operatorString switch
        {
            "=" => FilterOperator.Equals,
            "!=" => FilterOperator.NotEquals,
            ">" => FilterOperator.GreaterThan,
            "<" => FilterOperator.LessThan,
            ">=" => FilterOperator.GreaterOrEqual,
            "<=" => FilterOperator.LessOrEqual,
            "=*" => FilterOperator.Contains,
            "!*" => FilterOperator.NotContains,
            "^" => FilterOperator.StartsWith,
            "!^" => FilterOperator.NotStartsWith,
            "$" => FilterOperator.EndsWith,
            "!$" => FilterOperator.NotEndsWith,
            _ => null
        };
    }

    private static string? ParseValue(string value, FilterPropertyType propertyType)
    {
        if (string.IsNullOrEmpty(value))
        {
            return null;
        }

        switch (propertyType)
        {
            case FilterPropertyType.String:
                return value;

            case FilterPropertyType.Number:
                // Validate it's a valid number
                if (decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    return value;
                }
                return null;

            case FilterPropertyType.Boolean:
                // Accept true/false (case-insensitive)
                if (bool.TryParse(value, out _))
                {
                    return value.ToLowerInvariant();
                }
                return null;

            case FilterPropertyType.Date:
                // Validate date format (yyyy-MM-dd)
                if (DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
                {
                    return value;
                }
                return null;

            case FilterPropertyType.DateTime:
                // Validate datetime format (yyyy-MM-ddTHH:mm:ss)
                if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
                {
                    return value;
                }
                // Also try with milliseconds
                if (DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm:ss.fff", CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out _))
                {
                    return value;
                }
                return null;

            default:
                return null;
        }
    }
}
