namespace Crud.Generator.Abstractions;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}