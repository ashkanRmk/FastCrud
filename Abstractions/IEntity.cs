namespace FastCrud.Abstractions;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}