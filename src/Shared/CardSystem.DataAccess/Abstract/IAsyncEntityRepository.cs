using System.Linq.Expressions;
using CardSystem.Domain.Models;

namespace CardSystem.DataAccess.Abstract;

public interface IAsyncEntityRepository<TEntity, in TId> where TEntity : EntityBase<TId>
{
    Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> clause);
    Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? where = null);
    Task<TEntity?> AddAsync(TEntity entity);
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities);
    Task<TEntity?> UpdateAsync(TEntity entity);
    Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities);
    Task<bool> DeleteAsync(TId id);
}