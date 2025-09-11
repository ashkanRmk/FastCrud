using FastCrud.Abstractions.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FastCrud.Persistence.EFCore;

public class EfAuditQueryService<TAuditEntry> : IAuditQueryService<TAuditEntry>
    where TAuditEntry : class, IAuditEntry
{
    private readonly DbContext _context;

    public EfAuditQueryService(DbContext context)
    {
        _context = context;
    }

    public async Task<object> GetRecentAuditLogsAsync(int count = 100, CancellationToken cancellationToken = default)
    {
        var auditLogs = await _context.Set<TAuditEntry>()
            .OrderByDescending(x => x.Timestamp)
            .Take(count)
            .ToListAsync(cancellationToken);

        var logs = auditLogs.Select(log => new
        {
            Id = GetAuditEntryId(log),
            Entity = log.EntityName,
            EntityId = log.EntityId.Length > 8 ? log.EntityId[..8] : log.EntityId,
            Action = log.Action.ToString(),
            Timestamp = log.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
            User = $"{log.UserName ?? "Unknown"} ({log.UserId ?? "N/A"})",
            OldValues = log.OldValues,
            NewValues = log.NewValues
        });

        return new
        {
            TotalLogs = auditLogs.Count,
            Logs = logs
        };
    }

    private static object GetAuditEntryId(IAuditEntry entry)
    {
        var idProperty = entry.GetType().GetProperty("Id");
        return idProperty?.GetValue(entry) ?? 0;
    }
}