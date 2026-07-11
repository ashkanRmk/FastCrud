namespace FastCrud.Samples.Api.Dtos
{
    public record AuditLogDto(
       object Id,
       string Entity,
       string EntityId,
       string Action,
       string Timestamp,
       string User,
       string? OldValues,
       string? NewValues
   );

    public record AuditLogsResponse(int TotalLogs, IEnumerable<AuditLogDto> Logs);
}
