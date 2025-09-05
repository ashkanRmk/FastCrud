using FluentValidation;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Validation.FluentValidation
{
    /// <summary>
    /// Adapter that bridges FluentValidation validators to the <see cref="IModelValidator{T}"/> abstraction used by FastCrud.
    /// </summary>
    /// <typeparam name="T">Type being validated.</typeparam>
    /// <remarks>
    /// Initializes a new instance with the supplied FluentValidation validator.
    /// </remarks>
    /// <param name="validator">Concrete validator.</param>
    public sealed class FluentValidationModelValidator<T>(IValidator<T> validator) : IModelValidator<T>
    {
        /// <inheritdoc />
        public async Task ValidateAsync(T model, CancellationToken cancellationToken)
        {
            var result = await validator.ValidateAsync(model, cancellationToken);
            if (!result.IsValid)
            {
                // throw an exception with all validation errors
                throw new ValidationException(result.Errors);
            }
        }
    }
}
