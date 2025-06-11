using Asp.Versioning;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.WebApi.Common.Models;
using CleanArchitecture.WebApi.Controllers.Base;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v1/[controller]")]
public class ProductCategoriesController : ApiControllerBase<ProductCategoriesController>
{
    private readonly IProductCategoryService _categoryService;

    public ProductCategoriesController(
        ILogger<ProductCategoriesController> logger,
        Dictionary<Type, IValidator> validators,
        IProductCategoryService categoryService) : base(logger, validators)
    {
        _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _categoryService.GetAllAsync();
        return Ok(ApiResult<IEnumerable<ProductCategoryDto>>.SuccessResult(categories));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        await ValidateBaseEntity(id);
        var category = await _categoryService.GetByIdAsync(id);
        return Ok(ApiResult<ProductCategoryDto>.SuccessResult(category));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCategoryDto dto)
    {
        var userId = await ValidateTokenUserId();
        await ValidateRequest(dto);
        await _categoryService.AddAsync(dto, userId);
        return Ok(ApiResult<string>.SuccessResult("Product category created successfully"));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCategoryDto dto)
    {
        var userId = await ValidateTokenUserId();
        await ValidateBaseEntity(id);
        await ValidateRequest(dto);
        await _categoryService.UpdateAsync(dto, id, userId);
        return Ok(ApiResult<string>.SuccessResult("Product category updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = await ValidateTokenUserId();
        await _categoryService.DeleteAsync(id, userId);
        return Ok(ApiResult<string>.SuccessResult("Product category deleted successfully"));
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _categoryService.GetPagedAsync(page, pageSize);
        return Ok(ApiResult<PagedResult<ProductCategoryDto>>.SuccessResult(result));
    }
}