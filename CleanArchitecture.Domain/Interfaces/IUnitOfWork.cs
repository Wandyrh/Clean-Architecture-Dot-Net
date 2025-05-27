using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IUnitOfWork
{
    IGenericRepository<Product> Products { get; }
    IGenericRepository<ProductCategory> Categories { get; }

    Task<int> SaveChangesAsync();
}
