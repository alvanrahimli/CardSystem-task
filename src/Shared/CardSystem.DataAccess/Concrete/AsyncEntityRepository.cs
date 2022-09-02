using System.Linq.Expressions;
using CardSystem.DataAccess.Abstract;
using CardSystem.Domain.Data;
using CardSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CardSystem.DataAccess.Concrete;

public class AsyncEntityRepository<TEntity, TId> : IAsyncEntityRepository<TEntity, TId> where TEntity : EntityBase<TId>
{
    private readonly DataContext _context;
    private readonly DbSet<TEntity> _table;

    public AsyncEntityRepository(DataContext context)
    {
        _context = context;
        _table = context.Set<TEntity>();
    }
    
    public async Task<TEntity?> GetSingle(Expression<Func<TEntity, bool>> clause)
    {
        return await _table.FirstOrDefaultAsync(clause);
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? where = null)
    {
        return where is null
            ? await _table.ToListAsync()
            : await _table.Where(where).ToListAsync();
    }

    public async Task<TEntity?> AddAsync(TEntity entity)
    {
        return (await _table.AddAsync(entity)).Entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        var addedItems = entities.ToList();
        await _table.AddRangeAsync(addedItems);
        return addedItems;
    }

    public async Task<TEntity?> UpdateAsync(TEntity entity)
    {
        _table.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
        return await SaveChangesAsync() ? entity : null;
    }

    public async Task<List<TEntity>> UpdateRangeAsync(List<TEntity> entities)
    {
        _table.AttachRange(entities);
        entities.ForEach(e => _context.Entry(e).State = EntityState.Modified);
        return await SaveChangesAsync() ? entities : new List<TEntity>();
    }

    public async Task<bool> DeleteAsync(TId id)
    {
        var entity = await _table.FindAsync(id);
        if (entity is null) return true;
        
        _table.Remove(entity);
        return await SaveChangesAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}