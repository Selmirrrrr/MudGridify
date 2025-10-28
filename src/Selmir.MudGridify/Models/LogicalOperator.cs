namespace Selmir.MudGridify.Models;

/// <summary>
/// Logical operators for combining filter conditions
/// </summary>
public enum LogicalOperator
{
    And,  // , (comma)
    Or    // | (pipe)
}

/// <summary>
/// Extension methods for LogicalOperator
/// </summary>
public static class LogicalOperatorExtensions
{
    /// <summary>
    /// Converts LogicalOperator to Gridify operator string
    /// </summary>
    public static string ToGridifyOperator(this LogicalOperator op)
    {
        return op switch
        {
            LogicalOperator.And => ",",
            LogicalOperator.Or => "|",
            _ => ","
        };
    }

    /// <summary>
    /// Gets display name for the operator
    /// </summary>
    public static string GetDisplayName(this LogicalOperator op)
    {
        return op switch
        {
            LogicalOperator.And => "AND",
            LogicalOperator.Or => "OR",
            _ => "AND"
        };
    }
}
