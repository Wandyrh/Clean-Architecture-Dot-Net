using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Infrastructure.Persistence.Repositories;

namespace CleanArchitecture.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public IGenericRepository<Product> Products { get; }
    public IGenericRepository<ProductCategory> Categories { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Products = new GenericRepository<Product>(_context);
        Categories = new GenericRepository<ProductCategory>(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
