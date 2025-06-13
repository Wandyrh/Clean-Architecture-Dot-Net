using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.Caching.Memory;
using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Extensions;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Common;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;
    private readonly int _cacheMinutes;

    public ProductService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMemoryCache cache,
        IOptions<CacheSettings> cacheSettings)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        _cacheMinutes = cacheSettings?.Value?.TimeInMinutes ?? 10;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        string cacheKey = "products_all";
        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is IEnumerable<ProductDto> cachedProducts)
        {
            return cachedProducts;
        }

        var products = await _unitOfWork.Products.GetAllWithCategoryAsync();
        var dtoList = _mapper.Map<IEnumerable<ProductDto>>(products);

        _cache.Set(cacheKey, dtoList, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheMinutes)
        });

        return dtoList;
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        string cacheKey = $"product_{id}";
        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is ProductDto cachedProduct)
        {
            return cachedProduct;
        }

        var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(id);
        if (product is null)
            throw new NotFoundException(AppMessages.ProductNotFound);

        var dto = _mapper.Map<ProductDto>(product);
        _cache.Set(cacheKey, dto, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheMinutes)
        });
        return dto;
    }

    public async Task AddAsync(CreateProductDto dto, Guid userId)
    {
        var product = _mapper.Map<Product>(dto);
        product.CreatedAt = DateTime.UtcNow;
        product.CreatedBy = userId;
        await _unitOfWork.Products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove("products_all");
        RemovePagedProductCaches();
    }

    public async Task UpdateAsync(UpdateProductDto dto, Guid id, Guid userId)
    {
        if (id != dto.Id)
            throw new IDMismatchException(AppMessages.ProductIdMismatch);

        var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
        if (product is null)
            throw new NotFoundException(AppMessages.ProductNotFound);

        product.ModifiedAt = DateTime.UtcNow;
        product.ModifiedBy = userId;
        _mapper.Map(dto, product);

        await _unitOfWork.Products.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove($"product_{id}");
        _cache.Remove("products_all");
        RemovePagedProductCaches();
    }



    public async Task DeleteAsync(Guid id, Guid userId)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product is null)
            throw new NotFoundException(AppMessages.ProductNotFound);
        product.DeletedAt = DateTime.UtcNow;
        product.DeletedBy = userId;

        await _unitOfWork.Products.RemoveAsync(product);
        await _unitOfWork.SaveChangesAsync();

        _cache.Remove($"product_{id}");
        _cache.Remove("products_all");
        RemovePagedProductCaches();
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(int page, int pageSize)
    {
        string cacheKey = $"products_paged_{page}_{pageSize}";
        if (_cache.TryGetValue(cacheKey, out var cachedObj) && cachedObj is PagedResult<ProductDto> cachedPaged)
        {
            return cachedPaged;
        }

        var query = _unitOfWork.Products
            .GetAll()
            .Include(p => p.Category)
            .AsNoTracking()
            .OrderBy(_ => _.Name)
            .ProjectTo<ProductDto>(_mapper.ConfigurationProvider);

        var pagedResult = await query.ToPagedResultAsync(page, pageSize);

        _cache.Set(cacheKey, pagedResult, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheMinutes)
        });

        return pagedResult;
    }

    private void RemovePagedProductCaches()
    {
        if (_cache is MemoryCache memCache)
        {
            var entries = memCache.GetType()
                .GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(memCache) as System.Collections.ICollection;

            if (entries != null)
            {
                var keysToRemove = new List<object>();
                foreach (var entry in entries)
                {
                    var entryType = entry.GetType();
                    var keyProp = entryType.GetProperty("Key");
                    var key = keyProp?.GetValue(entry);
                    if (key is string strKey && strKey.StartsWith("products_paged_"))
                    {
                        keysToRemove.Add(key);
                    }
                }
                foreach (var key in keysToRemove)
                {
                    memCache.Remove(key);
                }
            }
        }
    }
}