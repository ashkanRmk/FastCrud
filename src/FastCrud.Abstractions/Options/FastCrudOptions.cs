namespace FastCrud.Abstractions.Options;

public sealed class FastCrudOptions
{
    public bool SoftDeleteEnabled { get; set; } = true;
    public bool AuditEnabled { get; set; } = false;
}
