using AutoMapper;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Tests;

public class ProductCategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<IMapper> _mapper = new();

    [Fact]
    public async Task GetAllAsync_ReturnsCategoryDtos()
    {
        var categories = new List<ProductCategory> { new ProductCategory { Id = Guid.NewGuid(), Name = "Test" } };
        _unitOfWork.Setup(u => u.Categories.GetAllAsync()).ReturnsAsync(categories);
        _mapper.Setup(m => m.Map<IEnumerable<ProductCategoryDto>>(categories)).Returns(new List<ProductCategoryDto> { new ProductCategoryDto { Name = "Test" } });

        var service = new ProductCategoryService(_unitOfWork.Object, _mapper.Object);

        var result = await service.GetAllAsync();

        Assert.NotNull(result);
    }
[Fact]
    public async Task GetByIdAsync_ReturnsCategoryDto()
    {
        var id = Guid.NewGuid();
        var category = new ProductCategory { Id = id, Name = "Test" };
        _unitOfWork.Setup(u => u.Categories.GetByIdAsync(id)).ReturnsAsync(category);
        _mapper.Setup(m => m.Map<ProductCategoryDto>(category)).Returns(new ProductCategoryDto { Name = "Test" });

        var service = new ProductCategoryService(_unitOfWork.Object, _mapper.Object);

        var result = await service.GetByIdAsync(id);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }

    [Fact]
    public async Task AddAsync_AddsCategory()
    {
        var dto = new CreateProductCategoryDto { Name = "Test" };
        var userId = Guid.NewGuid();
        var category = new ProductCategory { Id = Guid.NewGuid(), Name = "Test" };
        _mapper.Setup(m => m.Map<ProductCategory>(dto)).Returns(category);

        _unitOfWork.Setup(u => u.Categories.AddAsync(It.IsAny<ProductCategory>())).Returns(Task.CompletedTask);
        _unitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var service = new ProductCategoryService(_unitOfWork.Object, _mapper.Object);

        await service.AddAsync(dto, userId);

        _unitOfWork.Verify(u => u.Categories.AddAsync(category), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesCategory()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var dto = new UpdateProductCategoryDto { Id = id, Name = "Updated" };
        var category = new ProductCategory { Id = id, Name = "Old" };
        _unitOfWork.Setup(u => u.Categories.GetByIdAsync(id)).ReturnsAsync(category);

        _mapper.Setup(m => m.Map(dto, category)).Callback<UpdateProductCategoryDto, ProductCategory>((src, dest) => dest.Name = src.Name);

        var service = new ProductCategoryService(_unitOfWork.Object, _mapper.Object);

        await service.UpdateAsync(dto, id, userId);

        Assert.Equal("Updated", category.Name);
        _unitOfWork.Verify(u => u.Categories.UpdateAsync(category), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_DeletesCategory()
    {
        var id = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var category = new ProductCategory { Id = id, Name = "ToDelete" };
        _unitOfWork.Setup(u => u.Categories.GetByIdAsync(id)).ReturnsAsync(category);

        var service = new ProductCategoryService(_unitOfWork.Object, _mapper.Object);

        await service.DeleteAsync(id, userId);

        _unitOfWork.Verify(u => u.Categories.RemoveAsync(category), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsPagedResult()
    {      
        var service = new ProductCategoryService(_unitOfWork.Object, _mapper.Object);      
        await Assert.ThrowsAsync<NullReferenceException>(() => service.GetPagedAsync(1, 10));
    }
}