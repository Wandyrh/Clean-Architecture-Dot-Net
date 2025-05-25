using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task AddAsync(CreateProductDto product);
    Task UpdateAsync(UpdateProductDto product);
    Task DeleteAsync(Guid id);
}
