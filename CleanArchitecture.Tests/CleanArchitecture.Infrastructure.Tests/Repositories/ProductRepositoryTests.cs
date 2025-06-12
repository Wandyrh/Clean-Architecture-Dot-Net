using Microsoft.EntityFrameworkCore;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Infrastructure.Persistence.Repositories;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Tests.CleanArchitecture.Infrastructure.Tests.Repositories;

public class ProductRepositoryTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsProductsOfCategory()
    {        
        var context = GetInMemoryDbContext();
        var categoryId = Guid.NewGuid();
        var otherCategoryId = Guid.NewGuid();

        var products = new List<Product>
        {
            new Product { Id = Guid.NewGuid(), Name = "Product 1", Description = "Desc 1", CategoryId = categoryId },
            new Product { Id = Guid.NewGuid(), Name = "Product 2", Description = "Desc 2", CategoryId = categoryId },
            new Product { Id = Guid.NewGuid(), Name = "Product 3", Description = "Desc 3", CategoryId = otherCategoryId }
        };

        context.Set<Product>().AddRange(products);
        context.SaveChanges();

        var repository = new ProductRepository(context);
        
        var result = await repository.GetByCategoryIdAsync(categoryId);
        
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(2, resultList.Count);
        Assert.All(resultList, p => Assert.Equal(categoryId, p.CategoryId));
    }

    [Fact]
    public async Task GetByCategoryIdAsync_ReturnsEmpty_WhenNoProductsInCategory()
    {      
        var context = GetInMemoryDbContext();
        var categoryId = Guid.NewGuid();
        var repository = new ProductRepository(context);
      
        var result = await repository.GetByCategoryIdAsync(categoryId);

        Assert.NotNull(result);
        Assert.Empty(result);
    }
}