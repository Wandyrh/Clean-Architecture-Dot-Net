namespace CleanArchitecture.Domain.Interfaces;

public interface IUnitOfWork
{
    IProductRepository Products { get; }
    IProductCategoryRepository Categories { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync();
}
