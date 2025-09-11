using FastCrud.Abstractions.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FastCrud.Web.MinimalApi;

public static class AuditEndpointExtensions
{
    public static IEndpointRouteBuilder MapAuditLogs<TAuditEntry>(
        this IEndpointRouteBuilder builder,
        string routePrefix = "/api/audit-logs",
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

        return builder;
    }
}