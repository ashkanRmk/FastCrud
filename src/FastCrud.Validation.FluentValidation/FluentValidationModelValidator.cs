using FluentValidation;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Validation.FluentValidation;

public sealed class FluentValidationModelValidator<T>(
    IEnumerable<IValidator<T>> validators
) : IModelValidator<T>
{
    public async Task ValidateAsync(T model, CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(model, cancellationToken);
            if (!result.IsValid)
                throw new ValidationException(result.Errors);
        }
    }
}