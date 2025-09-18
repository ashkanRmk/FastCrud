using FastCrud.Abstractions.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FastCrud.Persistence.EFCore.DI;

public static class AuditServiceCollectionExtensions
{
    public static IServiceCollection AddEfAuditing<TDbContext, TAuditEntry>(this IServiceCollection services)
        where TDbContext : DbContext
        where TAuditEntry : class, IAuditEntry, new()
    {
        services.AddScoped<IAuditUserProvider, DefaultAuditUserProvider>();
        services.AddScoped<EntityAuditingInterceptor<TAuditEntry>>();
        services.AddScoped<IAuditService>(sp =>
            new EfAuditService<TDbContext, TAuditEntry>(
                sp.GetRequiredService<TDbContext>(),
                sp.GetRequiredService<IAuditUserProvider>()));

        services.AddScoped<IAuditQueryService<TAuditEntry>>(sp =>
            new EfAuditQueryService<TAuditEntry>(sp.GetRequiredService<TDbContext>()));

        return services;
    }

    public static IServiceCollection AddCustomAuditUserProvider<TProvider>(this IServiceCollection services)
        where TProvider : class, IAuditUserProvider
    {
        services.AddScoped<IAuditUserProvider, TProvider>();
        return services;
    }

    public static IServiceCollection AddEfAuditing<TDbContext>(this IServiceCollection services,
        Type auditEntryType)
        where TDbContext : DbContext
    {
        if (!typeof(IAuditEntry).IsAssignableFrom(auditEntryType))
        {
            throw new ArgumentException($"Type: {auditEntryType.Name} ans IAuditEntry", nameof(auditEntryType));
        }

        services.AddScoped<IAuditUserProvider, DefaultAuditUserProvider>();
        var interceptorType = typeof(EntityAuditingInterceptor<>).MakeGenericType(auditEntryType);
        services.AddScoped(interceptorType);

        services.AddScoped<IAuditService>(sp =>
        {
            var auditServiceType = typeof(EfAuditService<,>).MakeGenericType(typeof(TDbContext), auditEntryType);
            return (IAuditService)Activator.CreateInstance(auditServiceType,
                sp.GetRequiredService<TDbContext>(),
                sp.GetRequiredService<IAuditUserProvider>())!;
        });

        return services;
    }
}