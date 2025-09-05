namespace FastCrud.Abstractions.Primitives;
public sealed record PagedResult<T>(IReadOnlyList<T> Items, long TotalItems, int Page, int PageSize);
// public sealed record ValidationError(string Field, string Message);
// public sealed record ValidationResult(bool IsValid, IReadOnlyList<ValidationError> Errors);
