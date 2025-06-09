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
public class UsersController : ApiControllerBase<UsersController>
{
    private readonly IUserService _UserService;

    public UsersController(ILogger<UsersController> logger,
        Dictionary<Type, IValidator> validators,
        IUserService UserService) : base(logger, validators)
    {
        _UserService = UserService ?? throw new ArgumentNullException(nameof(UserService));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _UserService.GetAllAsync();
        return Ok(ApiResult<IEnumerable<UserDto>>.SuccessResult(users));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        await ValidateBaseEntity(id);
        var user = await _UserService.GetByIdAsync(id);
        return Ok(ApiResult<UserDto>.SuccessResult(user));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        await ValidateRequest(dto);
        await _UserService.AddAsync(dto);
        return Ok(ApiResult<string>.SuccessResult("User created successfully"));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserDto dto)
    {
        await ValidateBaseEntity(id);
        await ValidateRequest(dto);
        await _UserService.UpdateAsync(dto, id);
        return Ok(ApiResult<string>.SuccessResult("User updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await ValidateBaseEntity(id);
        await _UserService.DeleteAsync(id);
        return Ok(ApiResult<string>.SuccessResult("User deleted successfully"));
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var result = await _UserService.GetPagedAsync(page, pageSize);
        return Ok(ApiResult<PagedResult<UserDto>>.SuccessResult(result));
    }
}
