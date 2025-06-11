using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(Guid id);    
    Task AddAsync(CreateProductDto product, Guid userId);
    Task UpdateAsync(UpdateProductDto product, Guid id, Guid userId);   
    Task DeleteAsync(Guid id, Guid userId);
    Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize);
}
