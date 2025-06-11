using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface IProductCategoryService
{
    Task<IEnumerable<ProductCategoryDto>> GetAllAsync();
    Task<ProductCategoryDto> GetByIdAsync(Guid id);
    Task AddAsync(CreateProductCategoryDto category);
    Task UpdateAsync(UpdateProductCategoryDto category, Guid id);
    Task DeleteAsync(Guid id);
    Task<PagedResult<ProductCategoryDto>> GetPagedAsync(int page, int pageSize);
}