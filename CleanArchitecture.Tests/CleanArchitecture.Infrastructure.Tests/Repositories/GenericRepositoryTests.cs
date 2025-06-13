using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Tests.CleanArchitecture.Infrastructure.Tests.Repositories;

public class GenericRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsEntity()
    {
        var context = GetInMemoryDbContext();
        var repo = new GenericRepository<Product>(context);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "TestProduct",
            Description = "Test Description",
            CategoryId = Guid.NewGuid()
        };

        await repo.AddAsync(product);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(product.Id);
        Assert.NotNull(found);
        Assert.Equal("TestProduct", found.Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        var context = GetInMemoryDbContext();
        var repo = new GenericRepository<Product>(context);
        await repo.AddAsync(new Product
        {
            Id = Guid.NewGuid(),
            Name = "P1",
            Description = "Desc1",
            CategoryId = Guid.NewGuid()
        });
        await repo.AddAsync(new Product
        {
            Id = Guid.NewGuid(),
            Name = "P2",
            Description = "Desc2",
            CategoryId = Guid.NewGuid()
        });
        await context.SaveChangesAsync();

        var all = await repo.GetAllAsync();
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task UpdateAsync_UpdatesEntity()
    {
        var context = GetInMemoryDbContext();
        var repo = new GenericRepository<Product>(context);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "OldName",
            Description = "OldDesc",
            CategoryId = Guid.NewGuid()
        };
        await repo.AddAsync(product);
        await context.SaveChangesAsync();

        product.Name = "NewName";
        product.Description = "NewDesc";
        await repo.UpdateAsync(product);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(product.Id);
        Assert.Equal("NewName", found.Name);
        Assert.Equal("NewDesc", found.Description);
    }

    [Fact]
    public async Task RemoveAsync_SoftDeletesEntity_WhenISoftDeletable()
    {
        var context = GetInMemoryDbContext();
        var repo = new GenericRepository<Product>(context);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "ToRemove",
            Description = "Desc",
            CategoryId = Guid.NewGuid()
        };
        await repo.AddAsync(product);
        await context.SaveChangesAsync();

        await repo.RemoveAsync(product);
        await context.SaveChangesAsync();
       
        var found = await context.Set<Product>().IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == product.Id);
        Assert.NotNull(found);
        var isSoftDeletable = found as ISoftDeletable;
        Assert.NotNull(isSoftDeletable);
        Assert.True(isSoftDeletable.IsDeleted);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueIfExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new GenericRepository<Product>(context);
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Exists",
            Description = "Desc",
            CategoryId = Guid.NewGuid()
        };
        await repo.AddAsync(product);
        await context.SaveChangesAsync();

        var exists = await repo.ExistsAsync(product.Id);
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalseIfNotExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new GenericRepository<Product>(context);

        var exists = await repo.ExistsAsync(Guid.NewGuid());
        Assert.False(exists);
    }
}