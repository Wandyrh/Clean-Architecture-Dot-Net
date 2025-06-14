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
using CleanArchitecture.Application.Common;

namespace CleanArchitecture.Application.Services;

public class ProductCategoryService : IProductCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductCategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ProductCategoryDto>> GetAllAsync()
    {
        var categories = await _unitOfWork.Categories.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductCategoryDto>>(categories);
    }

    public async Task<ProductCategoryDto> GetByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category is null)
            throw new NotFoundException(AppMessages.ProductCategoryNotFound);

        return _mapper.Map<ProductCategoryDto>(category);
    }

    public async Task AddAsync(CreateProductCategoryDto dto, Guid userId)
    {
        var category = _mapper.Map<ProductCategory>(dto);
        category.CreatedAt = DateTime.UtcNow;
        category.CreatedBy = userId;

        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UpdateAsync(UpdateProductCategoryDto dto, Guid id, Guid userId)
    {
        if (id != dto.Id)
            throw new IDMismatchException(AppMessages.ProductCategoryIdMismatch);

        var category = await _unitOfWork.Categories.GetByIdAsync(dto.Id);
        if (category is null)
            throw new NotFoundException(AppMessages.ProductCategoryNotFound);

        category.ModifiedAt = DateTime.UtcNow;
        category.ModifiedBy = userId;
        _mapper.Map(dto, category);

        await _unitOfWork.Categories.UpdateAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category is null)
            throw new NotFoundException(AppMessages.ProductCategoryNotFound);
        
        category.DeletedAt = DateTime.UtcNow;
        category.DeletedBy = userId;

        await _unitOfWork.Categories.RemoveAsync(category);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<PagedResult<ProductCategoryDto>> GetPagedAsync(int page, int pageSize)
    {
        var query = _unitOfWork.Categories
            .GetAll()
            .AsNoTracking()
            .OrderBy(_ => _.Name)
            .ProjectTo<ProductCategoryDto>(_mapper.ConfigurationProvider);

        return await query.ToPagedResultAsync(page, pageSize);
    }
}