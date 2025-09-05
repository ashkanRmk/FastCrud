namespace FastCrud.Abstractions.Abstractions;

public interface IModelValidator<in T>
{
    Task ValidateAsync(T model, CancellationToken cancellationToken);
}