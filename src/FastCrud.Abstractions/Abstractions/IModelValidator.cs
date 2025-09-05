namespace FastCrud.Abstractions.Abstractions;

/// <summary>
/// Abstraction over a model validator. Implementations validate domain entities using various validation libraries.
/// </summary>
public interface IModelValidator<T>
{
    /// <summary>
    /// Validates the supplied model and throws an exception if validation fails.
    /// </summary>
    /// <param name="model">Model to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task ValidateAsync(T model, CancellationToken cancellationToken);
}