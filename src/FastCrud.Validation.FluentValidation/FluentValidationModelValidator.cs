using FluentValidation;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Validation.FluentValidation;

public sealed class FluentValidationModelValidator<T>(
    IEnumerable<IValidator<T>> validators
) : IModelValidator<T>
{
    public async Task<(bool ok, string message)> ValidateAsync(T model, CancellationToken cancellationToken)
    {
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(model, cancellationToken);
            if (!result.IsValid)
                return (false, result.ToString());

        }
        return (true, string.Empty);
    }
}