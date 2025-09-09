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
            var callingAssembly = Assembly.GetExecutingAssembly();
            services.AddValidatorsFromAssembly(callingAssembly);
        }

        services.AddScoped(typeof(IModelValidator<>), typeof(FluentValidationModelValidator<>));
        return services;
    }
}