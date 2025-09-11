using FastCrud.Abstractions.Abstractions;

namespace FastCrud.Persistence.EFCore;

public sealed class DefaultAuditUserProvider : IAuditUserProvider
{
    public (string? UserId, string? UserName) GetCurrentUser()
    {
        return ("system", "System");
    }
}