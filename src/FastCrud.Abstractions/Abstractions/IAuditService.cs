using FastCrud.Abstractions.Primitives;

namespace FastCrud.Abstractions.Abstractions
{
    public interface IAuditService
    {
        Task LogAsync<T>(T entity, AuditAction action, object? oldValues = null, object? newValues = null, CancellationToken cancellationToken = default);
    }
}
