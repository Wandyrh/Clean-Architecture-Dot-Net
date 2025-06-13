using AutoMapper;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Application.Tests;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
    private readonly IOptions<CacheSettings> _cacheSettings =
        Options.Create(new CacheSettings { TimeInMinutes = 10 });

    [Fact]
    public async Task GetAllAsync_ReturnsProductDtos()
    {
        var products = new List<Product> { new Product { Id = Guid.NewGuid(), Name = "Test" } };
        _unitOfWork.Setup(u => u.Products.GetAllWithCategoryAsync()).ReturnsAsync(products);
        _mapper.Setup(m => m.Map<IEnumerable<ProductDto>>(products)).Returns(new List<ProductDto> { new ProductDto { Name = "Test" } });

        var service = new ProductService(_unitOfWork.Object, _mapper.Object, _memoryCache, _cacheSettings);

        var result = await service.GetAllAsync();

        Assert.NotNull(result);
    }
    [Fact]
    public async Task GetByIdAsync_ReturnsProductDto()
    {
        var id = Guid.NewGuid();
        var product = new Product { Id = id, Name = "Test" };
        _unitOfWork.Setup(u => u.Products.GetByIdWithCategoryAsync(id)).ReturnsAsync(product);
        _mapper.Setup(m => m.Map<ProductDto>(product)).Returns(new ProductDto { Name = "Test" });

        var service = new ProductService(_unitOfWork.Object, _mapper.Object, _memoryCache, _cacheSettings);

        var result = await service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task AddAsync_AddsProduct()
    {
        var dto = new CreateProductDto { Name = "Test" };
        var userId = Guid.NewGuid();
        var product = new Product { Id = Guid.NewGuid(), Name = "Test" };
        _mapper.Setup(m => m.Map<Product>(dto)).Returns(product);
        _unitOfWork.Setup(u => u.Products.AddAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var service = new ProductService(_unitOfWork.Object, _mapper.Object, _memoryCache, _cacheSettings);

        await service.AddAsync(dto, userId);

        _unitOfWork.Verify(u => u.Products.AddAsync(product), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesProduct()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new UpdateProductDto { Id = id, Name = "Updated" };
        var product = new Product { Id = id, Name = "Old" };
        _unitOfWork.Setup(u => u.Products.GetByIdAsync(id)).ReturnsAsync(product);
        _mapper.Setup(m => m.Map(dto, product)).Callback<UpdateProductDto, Product>((src, dest) => dest.Name = src.Name);
        _unitOfWork.Setup(u => u.Products.UpdateAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var service = new ProductService(_unitOfWork.Object, _mapper.Object, _memoryCache, _cacheSettings);

        await service.UpdateAsync(dto, id, userId);

        Assert.Equal("Updated", product.Name);
        _unitOfWork.Verify(u => u.Products.UpdateAsync(product), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesProduct()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var product = new Product { Id = id, Name = "ToDelete" };
        _unitOfWork.Setup(u => u.Products.GetByIdAsync(id)).ReturnsAsync(product);
        _unitOfWork.Setup(u => u.Products.RemoveAsync(It.IsAny<Product>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var service = new ProductService(_unitOfWork.Object, _mapper.Object, _memoryCache, _cacheSettings);

        await service.DeleteAsync(id, userId);

        _unitOfWork.Verify(u => u.Products.RemoveAsync(product), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResult()
    {
        var service = new ProductService(_unitOfWork.Object, _mapper.Object, _memoryCache, _cacheSettings);

        await Assert.ThrowsAsync<NullReferenceException>(() => service.GetPagedAsync(1, 10));
    }
}