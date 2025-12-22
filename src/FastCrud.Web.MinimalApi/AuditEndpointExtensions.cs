using FastCrud.Abstractions.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Reflection;

namespace FastCrud.Web.MinimalApi;

public static class AuditEndpointExtensions
{
    public static IEndpointRouteBuilder MapAuditLogs<TAuditEntry, TDbContext>(
        this IEndpointRouteBuilder builder,
        string routePrefix = "/api/audit-logs",
        IEnumerable<Type>? includeEntities = null,
        IEnumerable<Type>? excludeEntities = null,
        string? tagName = null,
        string? groupName = null)
        where TAuditEntry : class, IAuditEntry
    {
        tagName ??= "Audit";
        var prefix = routePrefix.StartsWith('/') ? routePrefix : $"/{routePrefix}";
        var group = builder.MapGroup(prefix).WithTags(tagName);

        if (groupName != null)
        {
            group.WithGroupName(groupName);
        }

        group.MapGet("/", async (
            IAuditQueryService<TAuditEntry> auditService,
            CancellationToken ct) =>
        {
            var result = await auditService.GetRecentAuditLogsAsync(100, ct);
            return Results.Ok(result);
        });

        var auditableEntities = GetAuditableEntities<TDbContext, TAuditEntry>(includeEntities, excludeEntities);

        foreach (var entityName in auditableEntities)
        {
            var entityRoute = $"/{entityName.ToLowerInvariant()}";
            group.MapGet(entityRoute, async (
                IAuditQueryService<TAuditEntry> auditService,
                CancellationToken ct) =>
            {
                var result = await auditService.GetAuditLogsByEntityAsync(entityName, 100, ct);
                return Results.Ok(result);
            })
            .WithName($"GetAuditLogsFor{entityName}");
        }

        return builder;
    }

    public static IEndpointRouteBuilder MapAuditLogsFor<TAuditEntry, TDbContext>(
        this IEndpointRouteBuilder builder,
        string routePrefix,
        string? tagName = null,
        string? groupName = null,
        params Type[] entityTypes)
        where TAuditEntry : class, IAuditEntry
    {
        return MapAuditLogs<TAuditEntry, TDbContext>(
            builder,
            routePrefix,
            includeEntities: entityTypes.Length > 0 ? entityTypes : null,
            excludeEntities: null,
            tagName: tagName,
            groupName: groupName);
    }

    private static IEnumerable<string> GetAuditableEntities<TDbContext, TAuditEntry>(
        IEnumerable<Type>? includeEntities = null,
        IEnumerable<Type>? excludeEntities = null)
        where TAuditEntry : class, IAuditEntry
    {
        var dbContextType = typeof(TDbContext);
        var auditEntryType = typeof(TAuditEntry);
        var iAuditEntryType = typeof(IAuditEntry);

        var allEntityTypes = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType &&
                       p.PropertyType.GetGenericTypeDefinition().FullName == "Microsoft.EntityFrameworkCore.DbSet`1")
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .Where(entityType =>
                !entityType.IsAssignableTo(iAuditEntryType) &&
                entityType != auditEntryType &&
                entityType.GetProperty("Id") != null)
            .ToList();

        if (includeEntities != null && includeEntities.Any())
        {
            var includeSet = new HashSet<Type>(includeEntities);
            allEntityTypes = allEntityTypes
                .Where(t => includeSet.Contains(t))
                .ToList();
        }

        if (excludeEntities != null && excludeEntities.Any())
        {
            var excludeSet = new HashSet<Type>(excludeEntities);
            allEntityTypes = allEntityTypes
                .Where(t => !excludeSet.Contains(t))
                .ToList();
        }

        return allEntityTypes
            .Select(entityType => entityType.Name)
            .Distinct()
            .OrderBy(name => name);
    }
}