using AutoMapper;
using AutoMapper.QueryableExtensions;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Extensions;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
            throw new NotFoundException("Product was not found");

        return _mapper.Map<ProductDto>(product);
    }

    public async Task AddAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        product.CreatedAt = DateTime.UtcNow;
        product.CreatedBy = Guid.NewGuid();
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateProductDto dto, Guid id)
    {
        if (id != dto.Id)
            throw new IDMismatchException("ID mismatch");

        var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
        if (product is null)
            throw new NotFoundException($"Product {dto.Id} was not found");

        _mapper.Map(dto, product);

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
            throw new NotFoundException("Product was not found");

        await _unitOfWork.Products.RemoveAsync(product);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize)
    {
        var query = _unitOfWork.Products
            .GetAll()
            .AsNoTracking()
            .OrderBy(_ => _.Name)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider);

        return await query.ToPagedResultAsync(page, pageSize);
    }
}