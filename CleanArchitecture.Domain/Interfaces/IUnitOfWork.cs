namespace CleanArchitecture.Domain.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IProductCategoryRepository Categories { get; }

    Task<int> SaveChangesAsync();
}
