﻿using System.Linq.Expressions;

namespace CleanArchitecture.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);

    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);

    Task RemoveAsync(T entity);
    Task RemoveRangeAsync(IEnumerable<T> entities);

    Task<bool> ExistsAsync(Guid id);
    Task<int> CountAsync();
    IQueryable<T> GetAll();
}
