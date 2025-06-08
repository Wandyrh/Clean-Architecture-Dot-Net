using Asp.Versioning;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.WebApi.Common.Models;
using CleanArchitecture.WebApi.Controllers.Base;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v1/[controller]")]
public class ProductsController : ApiControllerBase<ProductsController>
{
    private readonly IProductService _productService;   

    public ProductsController(ILogger<ProductsController> logger,
        Dictionary<Type, IValidator> validators,
        IProductService productService) : base(logger, validators)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));        
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();
        return Ok(ApiResult<IEnumerable<ProductDto>>.SuccessResult(products));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        await ValidateBaseEntity(id);
        var product = await _productService.GetByIdAsync(id);
        return Ok(ApiResult<ProductDto>.SuccessResult(product));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        await ValidateRequest(dto);
        await _productService.AddAsync(dto);
        return Ok(ApiResult<string>.SuccessResult("Product created successfully"));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        await ValidateBaseEntity(id);
        await ValidateRequest(dto);
        await _productService.UpdateAsync(dto, id);
        return Ok(ApiResult<string>.SuccessResult("Product updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _productService.DeleteAsync(id);
        return Ok(ApiResult<string>.SuccessResult("Product deleted successfully"));
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetPagedAsync(page, pageSize);
        return Ok(ApiResult<PagedResult<ProductDto>>.SuccessResult(result));
    }
}
