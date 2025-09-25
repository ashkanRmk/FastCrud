namespace FastCrud.Abstractions.Abstractions;

public interface IModelValidator<in T>
{
    Task<(bool ok, string message)> ValidateAsync(T model, CancellationToken cancellationToken);
}