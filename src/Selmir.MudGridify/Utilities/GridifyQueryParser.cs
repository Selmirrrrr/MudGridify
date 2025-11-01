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
    /// <returns>A list of parsed conditions with NextLogicalOperator set appropriately</returns>
    public static List<FilterCondition> Parse(
        string? gridifyQuery,
        List<FilterableProperty> filterableProperties)
    {
        var conditions = new List<FilterCondition>();

        if (string.IsNullOrWhiteSpace(gridifyQuery))
        {
            return conditions;
        }

        // We need to parse the query string while preserving the logical operators between conditions
        // The query can contain mixed operators like: "Name=John,Age>30|Dept=Sales,Status=Active"

        var currentPosition = 0;
        var queryLength = gridifyQuery.Length;

        while (currentPosition < queryLength)
        {
            // Find the next logical operator (, or |) or end of string
            var nextComma = gridifyQuery.IndexOf(',', currentPosition);
            var nextPipe = gridifyQuery.IndexOf('|', currentPosition);

            int segmentEnd;
            LogicalOperator? nextOperator = null;

            if (nextComma == -1 && nextPipe == -1)
            {
                // No more operators, this is the last segment
                segmentEnd = queryLength;
            }
            else if (nextComma == -1)
            {
                // Only pipe found
                segmentEnd = nextPipe;
                nextOperator = LogicalOperator.Or;
            }
            else if (nextPipe == -1)
            {
                // Only comma found
                segmentEnd = nextComma;
                nextOperator = LogicalOperator.And;
            }
            else
            {
                // Both found, use the closer one
                if (nextComma < nextPipe)
                {
                    segmentEnd = nextComma;
                    nextOperator = LogicalOperator.And;
                }
                else
                {
                    segmentEnd = nextPipe;
                    nextOperator = LogicalOperator.Or;
                }
            }

            // Extract and parse the segment
            var segment = gridifyQuery.Substring(currentPosition, segmentEnd - currentPosition);
            var condition = ParseCondition(segment.Trim(), filterableProperties);

            if (condition != null)
            {
                // Set the NextLogicalOperator if there's another condition after this one
                if (nextOperator.HasValue)
                {
                    condition.NextLogicalOperator = nextOperator.Value;
                }
                conditions.Add(condition);
            }

            // Move to the next segment (skip the operator)
            currentPosition = segmentEnd + 1;
        }

        return conditions;
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
        var match = Regex.Match(segment, @"^([a-zA-Z_][a-zA-Z0-9_]*)(!=|!\*|!\^|!\$|>=|<=|=\*|>|<|=|\^|\$)(.*)$");

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
