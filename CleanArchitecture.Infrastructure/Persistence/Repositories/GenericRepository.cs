using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly DbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();
    public IQueryable<T> GetAll() =>
         _dbSet.AsQueryable();

    public async Task<T?> GetByIdAsync(Guid id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();

    public async Task AddAsync(T entity) =>
        await _dbSet.AddAsync(entity);

    public async Task AddRangeAsync(IEnumerable<T> entities) =>
        await _dbSet.AddRangeAsync(entities);

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(T entity)
    {
        if (entity is ISoftDeletable deletableEntity)
        {
            deletableEntity.IsDeleted = true;
            _dbSet.Update(entity);
        }
        else
        {
            _dbSet.Remove(entity);
        }

        return Task.CompletedTask;
    }

    public Task RemoveRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var entity = await _dbSet.FindAsync(id);
        return entity != null;
    }

    public async Task<int> CountAsync() =>
        await _dbSet.CountAsync();
}


