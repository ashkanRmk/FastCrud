using FluentValidation;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Validation.FluentValidation
{
    public sealed class FluentValidationModelValidator<T>(IValidator<T> validator) : IModelValidator<T>
    {
        public async Task ValidateAsync(T model, CancellationToken cancellationToken)
        {
            var result = await validator.ValidateAsync(model, cancellationToken);
            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }
}
