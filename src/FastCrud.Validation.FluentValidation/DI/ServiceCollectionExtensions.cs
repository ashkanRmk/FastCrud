using System.Reflection;
using FastCrud.Abstractions.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FastCrud.Validation.FluentValidation.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseFluentValidationAdapter(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies is { Length: > 0 })
        {
            services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);
        }
        else
        {
            var entry  = Assembly.GetEntryAssembly();
            var loaded = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location));

            services.AddValidatorsFromAssemblies(
                new[] { entry! }.Concat(loaded).Distinct(),
                includeInternalTypes: true
            );
        }

        services.AddScoped(typeof(IModelValidator<>), typeof(FluentValidationModelValidator<>));
        // services.TryAddScoped(typeof(IModelValidator<>), typeof(FluentValidationDtoValidator<>)); 
        return services;
    }
}