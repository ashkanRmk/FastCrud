namespace FastCrud.Abstractions.Query;

public interface IQuerySpec
{
    int? Page { get; }
    int? PageSize { get; }
    IReadOnlyList<Sort> Sorts { get; }
    IReadOnlyList<Filter> Filters { get; }
}

public sealed record Sort(string Field, bool Descending);
public sealed record Filter(string Field, string Op, string? Value);
