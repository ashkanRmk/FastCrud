using FastCrud.Abstractions;
using global::FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace FastCrud.Validation.FluentValidation.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection UseFluentValidationAdapter(this IServiceCollection services, Assembly? assemblyToScan = null)
    {
        assemblyToScan ??= Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        services.AddValidatorsFromAssembly(assemblyToScan);
        services.AddTransient(typeof(IModelValidator<>), typeof(FluentValidationModelValidator<>));
        return services;
    }
}
