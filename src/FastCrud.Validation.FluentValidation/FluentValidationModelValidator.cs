using FastCrud.Abstractions;
using FastCrud.Abstractions.Primitives;
using global::FluentValidation;

namespace FastCrud.Validation.FluentValidation;

public sealed class FluentValidationModelValidator<T> : IModelValidator<T>
{
    private readonly IValidator<T> _inner;
    public FluentValidationModelValidator(IValidator<T> inner) => _inner = inner;

    public async Task<ValidationResult> ValidateAsync(T instance, CancellationToken ct = default)
    {
        var result = await _inner.ValidateAsync(instance, ct);
        if (result.IsValid) return ValidationResult.Success;
        var errs = result.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)).ToArray();
        return new ValidationResult(false, errs);
    }
}
