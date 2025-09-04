using FastCrud.Abstractions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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

    /// <summary>
    /// Service registration helpers for FluentValidation integration.
    /// </summary>
    public static class FluentValidationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers FluentValidation validators from the specified assemblies and configures them as the model validators for FastCrud.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="assemblies">Assemblies to scan for validators. If none provided, the calling assembly is used.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection UseFluentValidationAdapter(this IServiceCollection services, params Assembly[] assemblies)
        {
            // register FluentValidation validators
            if (assemblies != null && assemblies.Length > 0)
            {
                foreach (var assembly in assemblies)
                {
                    services.AddValidatorsFromAssembly(assembly);
                }
            }
            else
            {
                var callingAssembly = Assembly.GetCallingAssembly();
                services.AddValidatorsFromAssembly(callingAssembly);
            }
            // bridge IModelValidator<T> to FluentValidation
            services.AddTransient(typeof(IModelValidator<>), typeof(FluentValidationModelValidator<>));
            return services;
        }
    }
}
