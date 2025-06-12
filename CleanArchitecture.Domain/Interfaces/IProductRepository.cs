using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
    Task<IEnumerable<Product>> GetAllWithCategoryAsync();
    Task<Product?> GetByIdWithCategoryAsync(Guid id);
}