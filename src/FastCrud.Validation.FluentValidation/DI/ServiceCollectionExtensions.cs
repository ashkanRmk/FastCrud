using System.Reflection;
using FastCrud.Abstractions.Abstractions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Validation.FluentValidation.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseFluentValidationAdapter(this IServiceCollection services, params Assembly[] assemblies)
    {
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

        services.AddTransient(typeof(IModelValidator<>), typeof(FluentValidationModelValidator<>));
        return services;
    }
}