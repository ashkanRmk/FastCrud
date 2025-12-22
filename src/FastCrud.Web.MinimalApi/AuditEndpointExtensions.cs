using FastCrud.Abstractions.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FastCrud.Web.MinimalApi;

public static class AuditEndpointExtensions
{
    public static IEndpointRouteBuilder MapAuditLogs<TAuditEntry, TDbContext>(
        this IEndpointRouteBuilder builder,
        string routePrefix = "/api/audit-logs",
        string? tagName = null,
        string? groupName = null)
        where TAuditEntry : class, IAuditEntry
        where TDbContext : DbContext
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

        var auditableEntities = GetAuditableEntities<TDbContext, TAuditEntry>();
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

    private static IEnumerable<string> GetAuditableEntities<TDbContext, TAuditEntry>()
        where TDbContext : DbContext
        where TAuditEntry : class, IAuditEntry
    {
        var dbContextType = typeof(TDbContext);
        var auditEntryType = typeof(TAuditEntry);
        var iAuditEntryType = typeof(IAuditEntry);

        var dbSetProperties = dbContextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.PropertyType.IsGenericType &&
                       p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            .Select(p => p.PropertyType.GetGenericArguments()[0])
            .Where(entityType => 
                !entityType.IsAssignableTo(iAuditEntryType) &&
                entityType != auditEntryType &&
                entityType.GetProperty("Id") != null)
            .Select(entityType => entityType.Name)
            .Distinct()
            .OrderBy(name => name);

        return dbSetProperties;
    }
}