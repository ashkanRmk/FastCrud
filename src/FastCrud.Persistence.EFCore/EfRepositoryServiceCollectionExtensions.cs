using FastCrud.Abstractions;
using FastCrud.Abstractions.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Persistence.EFCore;

/// <summary>
/// Extensions for registering EF repositories with the DI container.
/// </summary>
public static class EfRepositoryServiceCollectionExtensions
{
    /// <summary>
    /// Registers an EF repository for the specified aggregate type and database context. When invoked, resolves
    /// <see cref="IRepository{TAgg, TId}"/> to an <see cref="EfRepository{TAgg, TId, TDbContext}"/>.
    /// </summary>
    /// <typeparam name="TAgg">Aggregate type.</typeparam>
    /// <typeparam name="TId">Identifier type.</typeparam>
    /// <typeparam name="TDbContext">DbContext type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
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