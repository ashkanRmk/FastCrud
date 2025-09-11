using FastCrud.Abstractions.Abstractions;
using FastCrud.Abstractions.Primitives;

namespace FastCrud.Samples.Api.Models
{
    public sealed class AuditEntry : IAuditEntry
    {
        public long Id { get; set; }
        public string EntityName { get; set; } = default!;
        public string EntityId { get; set; } = default!;
        public AuditAction Action { get; set; }
        public string? OldValues { get; set; }
        public string? NewValues { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
