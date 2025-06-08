using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Persistence.Repositories;

namespace CleanArchitecture.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IProductRepository Products { get; }
    public IProductCategoryRepository Categories { get; }
    public IUserRepository Users { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new ProductRepository(_context);
        Categories = new ProductCategoryRepository (_context);
        Users = new UserRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
