namespace FastCrud.Infrastructure;

public interface IEndpointModule
{
    string RoutePrefix { get; }
    string[] Tags => [RoutePrefix.Trim('/').ToUpperInvariant()];
    void Register(RouteGroupBuilder group);
}