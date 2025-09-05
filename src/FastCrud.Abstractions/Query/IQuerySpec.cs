namespace FastCrud.Abstractions.Query;

public interface IQuerySpec
{
    int Page { get; set; }
    int PageSize { get; set; }
    string? Filter { get; set; }
    string? OrderBy { get; set; }
}
