using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FastCrud.Persistence.EFCore;

public sealed class EfAuditService<TDbContext, TAuditEntry> : IAuditService
    where TDbContext : DbContext
    where TAuditEntry : class, IAuditEntry, new()
{
    private readonly TDbContext _context;
    private readonly IAuditUserProvider _userProvider;

    public EfAuditService(TDbContext context, IAuditUserProvider userProvider)
    {
        _context = context;
        _userProvider = userProvider;
    }

    public async Task LogAsync<T>(T entity, AuditAction action, object? oldValues = null, object? newValues = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var entityId = GetEntityId(entity);
            if (string.IsNullOrEmpty(entityId)) return;

            var user = _userProvider.GetCurrentUser();

            var auditEntry = new TAuditEntry
            {
                EntityName = typeof(T).Name,
                EntityId = entityId,
                Action = action,
                OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
                NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
                Timestamp = DateTime.UtcNow,
                UserId = user.UserId,
                UserName = user.UserName
            };

            _context.Set<TAuditEntry>().Add(auditEntry);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static string? GetEntityId<T>(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        return idProperty?.GetValue(entity)?.ToString();
    }
}