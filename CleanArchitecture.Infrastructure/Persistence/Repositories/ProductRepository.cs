using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
    {
        return await _dbSet.Where(p => p.CategoryId == categoryId).ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetAllWithCategoryAsync()
    {
        return await _dbSet.Include(p => p.Category).ToListAsync();
    }

    public async Task<Product?> GetByIdWithCategoryAsync(Guid id)
    {
        return await _dbSet.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    }
}
