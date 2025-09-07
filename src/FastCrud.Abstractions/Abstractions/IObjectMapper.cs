namespace FastCrud.Abstractions.Abstractions;

public interface IObjectMapper
{
    TDest Map<TDest>(object source);

    void Map(object source, object destination);
}