using System.Reflection;
using FastCrud.Abstractions.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Validation.FluentValidation;

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
        if (assemblies is { Length: > 0 })
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