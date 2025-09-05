using FluentValidation;
using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Validation.FluentValidation
{
    /// <summary>
    /// Adapter that bridges FluentValidation validators to the <see cref="IModelValidator{T}"/> abstraction used by FastCrud.
    /// </summary>
    /// <typeparam name="T">Type being validated.</typeparam>
    public sealed class FluentValidationModelValidator<T> : IModelValidator<T>
    {
        private readonly IValidator<T> _validator;
        /// <summary>
        /// Initializes a new instance with the supplied FluentValidation validator.
        /// </summary>
        /// <param name="validator">Concrete validator.</param>
        public FluentValidationModelValidator(IValidator<T> validator)
        {
            _validator = validator;
        }
        /// <inheritdoc />
        public async Task ValidateAsync(T model, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(model, cancellationToken);
            if (!result.IsValid)
            {
                // throw an exception with all validation errors
                throw new ValidationException(result.Errors);
            }
        }
    }
}
