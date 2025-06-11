using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface IProductCategoryService
{
    Task<IEnumerable<ProductCategoryDto>> GetAllAsync();
    Task<ProductCategoryDto> GetByIdAsync(Guid id);
    Task AddAsync(CreateProductCategoryDto category, Guid userId);
    Task UpdateAsync(UpdateProductCategoryDto category, Guid id, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<PagedResult<ProductCategoryDto>> GetPagedAsync(int page, int pageSize);
}