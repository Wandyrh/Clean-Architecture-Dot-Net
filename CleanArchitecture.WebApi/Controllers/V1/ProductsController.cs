using Asp.Versioning;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.WebApi.Common.Models;
using CleanArchitecture.WebApi.Common;
using CleanArchitecture.WebApi.Controllers.Base;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers.V1;

[Authorize]
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
        var userId = await ValidateTokenUserId();
        await ValidateRequest(dto);
        await _productService.AddAsync(dto, userId);
        return Ok(ApiResult<string>.SuccessResult(AppMessages.ProductCreated));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var userId = await ValidateTokenUserId();
        await ValidateBaseEntity(id);
        await ValidateRequest(dto);
        await _productService.UpdateAsync(dto, id, userId);
        return Ok(ApiResult<string>.SuccessResult(AppMessages.ProductUpdated));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = await ValidateTokenUserId();
        await _productService.DeleteAsync(id, userId);
        return Ok(ApiResult<string>.SuccessResult(AppMessages.ProductDeleted));
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _productService.GetPagedAsync(page, pageSize);
        return Ok(ApiResult<PagedResult<ProductDto>>.SuccessResult(result));
    }
}
