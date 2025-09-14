using FastCrud.Abstractions.Abstractions;
using FastCrud.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Core.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFastCrudCore(
            this IServiceCollection services)
        {
            services.AddScoped(typeof(ICrudService<,,,>), typeof(CrudService<,,,>));
            return services;
        }
    }
}