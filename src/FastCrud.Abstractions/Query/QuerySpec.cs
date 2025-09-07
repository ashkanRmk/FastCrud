namespace FastCrud.Abstractions.Query;

public sealed record QuerySpec : IQuerySpec
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
    public string? Filter { get; set; }
    public string? OrderBy { get; set; }
}