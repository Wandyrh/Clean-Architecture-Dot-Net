using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Domain.Interfaces;

public interface IUnitOfWork
{
    IRepository<Product> Products { get; }
    IRepository<ProductCategory> Categories { get; }

    Task<int> SaveChangesAsync();
}
