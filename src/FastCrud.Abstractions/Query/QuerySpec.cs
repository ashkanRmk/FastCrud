namespace FastCrud.Abstractions.Query;

/// <summary>
/// Default implementation of <see cref="IQuerySpec"/>. Used to capture paging, filtering and sorting parameters from query string binding.
/// </summary>
public class QuerySpec : IQuerySpec
{
    /// <inheritdoc />
    public int Page { get; set; } = 1;

    /// <inheritdoc />
    public int PageSize { get; set; } = 50;

    /// <inheritdoc />
    public string? Filter { get; set; }

    /// <inheritdoc />
    public string? OrderBy { get; set; }
}