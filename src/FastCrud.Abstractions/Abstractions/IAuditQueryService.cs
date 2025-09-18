namespace FastCrud.Abstractions.Abstractions
{
    public interface IAuditQueryService<TAuditEntry>
    where TAuditEntry : class, IAuditEntry
    {
        Task<object> GetRecentAuditLogsAsync(int count = 100, CancellationToken cancellationToken = default);
    }
}
