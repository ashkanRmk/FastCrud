using FastCrud.Abstractions.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Persistence.EFCore.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEfRepository<TAgg, TId, TDbContext>(this IServiceCollection services)
        where TAgg : class 
        where TDbContext : DbContext
    {
        services.AddScoped<IRepository<TAgg, TId>>(sp =>
        {
            var db = sp.GetRequiredService<TDbContext>();
            return new EfRepository<TAgg, TId, TDbContext>(db);
        });
        return services;
    }
}