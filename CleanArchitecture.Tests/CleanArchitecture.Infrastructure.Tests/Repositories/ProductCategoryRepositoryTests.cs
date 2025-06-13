using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;

namespace CleanArchitecture.Tests.CleanArchitecture.Infrastructure.Tests.Repositories;

public class ProductCategoryRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAsync_AddsCategory()
    {
        var context = GetInMemoryDbContext();
        var repo = new ProductCategoryRepository(context);
        var category = new ProductCategory { Id = Guid.NewGuid(), Name = "Category1", Description = "Desc1" };

        await repo.AddAsync(category);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(category.Id);
        Assert.NotNull(found);
        Assert.Equal("Category1", found.Name);
        Assert.Equal("Desc1", found.Description);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCategories()
    {
        var context = GetInMemoryDbContext();
        var repo = new ProductCategoryRepository(context);
        await repo.AddAsync(new ProductCategory { Id = Guid.NewGuid(), Name = "Cat1", Description = "Desc1" });
        await repo.AddAsync(new ProductCategory { Id = Guid.NewGuid(), Name = "Cat2", Description = "Desc2" });
        await context.SaveChangesAsync();

        var all = await repo.GetAllAsync();
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCategory()
    {
        var context = GetInMemoryDbContext();
        var repo = new ProductCategoryRepository(context);
        var category = new ProductCategory { Id = Guid.NewGuid(), Name = "OldName", Description = "OldDesc" };
        await repo.AddAsync(category);
        await context.SaveChangesAsync();

        category.Name = "NewName";
        category.Description = "NewDesc";
        await repo.UpdateAsync(category);
        await context.SaveChangesAsync();

        var found = await repo.GetByIdAsync(category.Id);
        Assert.Equal("NewName", found.Name);
        Assert.Equal("NewDesc", found.Description);
    }

    [Fact]
    public async Task RemoveAsync_SoftDeletesCategory_WhenISoftDeletable()
    {
        var context = GetInMemoryDbContext();
        var repo = new ProductCategoryRepository(context);
        var category = new ProductCategory { Id = Guid.NewGuid(), Name = "ToRemove", Description = "Desc" };
        await repo.AddAsync(category);
        await context.SaveChangesAsync();

        await repo.RemoveAsync(category);
        await context.SaveChangesAsync();
        
        var found = await context.Set<ProductCategory>().IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == category.Id);
        Assert.NotNull(found);
        var isSoftDeletable = found as ISoftDeletable;
        Assert.NotNull(isSoftDeletable);
        Assert.True(isSoftDeletable.IsDeleted);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueIfExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new ProductCategoryRepository(context);
        var category = new ProductCategory { Id = Guid.NewGuid(), Name = "Exists", Description = "Desc" };
        await repo.AddAsync(category);
        await context.SaveChangesAsync();

        var exists = await repo.ExistsAsync(category.Id);
        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsFalseIfNotExists()
    {
        var context = GetInMemoryDbContext();
        var repo = new ProductCategoryRepository(context);

        var exists = await repo.ExistsAsync(Guid.NewGuid());
        Assert.False(exists);
    }
}