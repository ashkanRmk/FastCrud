using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;

namespace FastCrud.Persistence.EFCore;

public sealed class EntityAuditingInterceptor<TAuditEntry> : SaveChangesInterceptor
    where TAuditEntry : class, IAuditEntry, new()
{
    private readonly IAuditUserProvider _userProvider;

    public EntityAuditingInterceptor(IAuditUserProvider userProvider)
    {
        _userProvider = userProvider;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        AddAuditEntries(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        AddAuditEntries(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddAuditEntries(DbContext? context)
    {
        if (context == null) return;

        try
        {
            var user = _userProvider.GetCurrentUser();
            var auditEntries = new List<TAuditEntry>();

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.Entity is IAuditEntry) continue;
                if (!HasIdProperty(entry.Entity)) continue;

                var auditEntry = CreateAuditEntry(entry, user);
                if (auditEntry != null)
                {
                    auditEntries.Add(auditEntry);
                }
                UpdateAuditableFields(entry, user);
            }

            if (auditEntries.Any())
            {
                context.Set<TAuditEntry>().AddRange(auditEntries);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static bool HasIdProperty(object entity)
    {
        return entity.GetType().GetProperty("Id") != null;
    }

    private TAuditEntry? CreateAuditEntry(EntityEntry entry, (string? UserId, string? UserName) user)
    {
        try
        {
            var entityName = entry.Entity.GetType().Name;
            var entityId = GetEntityId(entry);

            if (string.IsNullOrEmpty(entityId)) return null;

            var action = entry.State switch
            {
                EntityState.Added => AuditAction.Create,
                EntityState.Modified => AuditAction.Update,
                EntityState.Deleted => AuditAction.Delete,
                _ => (AuditAction?)null
            };

            if (action == null) return null;

            var auditEntry = new TAuditEntry
            {
                EntityName = entityName,
                EntityId = entityId,
                Action = action.Value,
                Timestamp = DateTime.UtcNow,
                UserId = user.UserId,
                UserName = user.UserName
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.NewValues = SerializeValues(entry, entry.CurrentValues);
                    break;
                case EntityState.Modified:
                    var modifiedProperties = entry.Properties
                        .Where(p => p.IsModified && !p.Metadata.IsPrimaryKey())
                        .ToList();

                    if (modifiedProperties.Any())
                    {
                        auditEntry.OldValues = SerializeModifiedValues(entry, entry.OriginalValues, modifiedProperties);
                        auditEntry.NewValues = SerializeModifiedValues(entry, entry.CurrentValues, modifiedProperties);
                    }
                    break;
                case EntityState.Deleted:
                    auditEntry.OldValues = SerializeValues(entry, entry.OriginalValues);
                    break;
            }

            return auditEntry;
        }
        catch
        {
            return null;
        }
    }

    private static void UpdateAuditableFields(EntityEntry entry, (string? UserId, string? UserName) user)
    {
        if (entry.Entity is not IAuditable auditable) return;

        var now = DateTime.UtcNow;
        var userName = user.UserName ?? user.UserId ?? "System";

        switch (entry.State)
        {
            case EntityState.Added:
                auditable.CreatedAt = now;
                auditable.CreatedBy = userName;
                break;
            case EntityState.Modified:
                auditable.UpdatedAt = now;
                auditable.UpdatedBy = userName;
                break;
        }
    }

    private static string? GetEntityId(EntityEntry entry)
    {
        var keyProperty = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
        return keyProperty?.CurrentValue?.ToString();
    }

    private static string? SerializeValues(EntityEntry entry, PropertyValues values)
    {
        try
        {
            var result = new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsForeignKey() ||
                    property.Metadata.IsShadowProperty() ||
                    property.Metadata.IsKey()) continue;

                var propertyName = property.Metadata.Name;
                var value = values[propertyName];

                if (value is DateTime dt)
                    value = dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                result[propertyName] = value;
            }

            return result.Any() ? JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) : null;
        }
        catch
        {
            return null;
        }
    }

    private static string? SerializeModifiedValues(EntityEntry entry, PropertyValues values, List<PropertyEntry> modifiedProperties)
    {
        try
        {
            var result = new Dictionary<string, object?>();

            foreach (var property in modifiedProperties)
            {
                var propertyName = property.Metadata.Name;
                var value = values[propertyName];

                if (value is DateTime dt)
                    value = dt.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");

                result[propertyName] = value;
            }

            return result.Any() ? JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }) : null;
        }
        catch
        {
            return null;
        }
    }
}