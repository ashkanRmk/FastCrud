namespace FastCrud.Abstractions.Query;

/// <summary>
/// Represents a query specification for paging, filtering and sorting.
/// </summary>
public interface IQuerySpec
{
    /// <summary>
    /// Gets or sets the page index (1-based).
    /// </summary>
    int Page { get; set; }

    /// <summary>
    /// Gets or sets the size of each page.
    /// </summary>
    int PageSize { get; set; }

    /// <summary>
    /// Gets or sets an optional filter string. Implementations may parse this string to construct filter expressions.
    /// </summary>
    string? Filter { get; set; }

    /// <summary>
    /// Gets or sets an optional sort expression. Comma-separated list of property names; prefix with '-' for descending order.
    /// </summary>
    string? OrderBy { get; set; }
}
