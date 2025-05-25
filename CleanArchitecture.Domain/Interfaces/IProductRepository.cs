using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
}