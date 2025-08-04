namespace Crud.Generator.Data;

public interface IRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity?> UpdateAsync(int id, TEntity entity);
    Task<bool> DeleteAsync(int id);
}