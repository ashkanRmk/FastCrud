using FastCrud.Abstractions.Primitives;

namespace FastCrud.Abstractions.Abstractions
{
    public interface IAuditEntry
    {
        string EntityName { get; set; }
        string EntityId { get; set; }
        AuditAction Action { get; set; }
        string? OldValues { get; set; }
        string? NewValues { get; set; }
        DateTime Timestamp { get; set; }
        string? UserId { get; set; }
        string? UserName { get; set; }
    }
}
