using FastCrud.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Persistence.EFCore.DI;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEfCoreRepositories<TDbContext>(this IServiceCollection services)
        where TDbContext : DbContext
    {
        services.AddScoped(typeof(IRepository<,>), typeof(EfRepository<,,>).MakeGenericType(typeof(object), typeof(object), typeof(TDbContext)));
        // Note: For open generics, you typically register concrete pairs per aggregate.
        // Example in composition root:
        // services.AddScoped<IRepository<Order, Guid>>(sp => new EfRepository<Order, Guid, OrdersDbContext>(sp.GetRequiredService<OrdersDbContext>()));
        return services;
    }
}
