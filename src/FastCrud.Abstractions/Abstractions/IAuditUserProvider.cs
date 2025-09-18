namespace FastCrud.Abstractions.Abstractions
{
    public interface IAuditUserProvider
    {
        (string? UserId, string? UserName) GetCurrentUser();
    }
}
