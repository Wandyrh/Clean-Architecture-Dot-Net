using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.WebApi.Common.Models;
using CleanArchitecture.WebApi.Controllers.V1;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CleanArchitecture.Tests.CleanArchitecture.WebApi.Tests.Controllers.V1;

public class UsersControllerTests
{
    private readonly Mock<ILogger<UsersController>> _loggerMock;
    private readonly Mock<IUserService> _serviceMock;
    private readonly Dictionary<Type, IValidator> _validators;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _loggerMock = new Mock<ILogger<UsersController>>();
        _serviceMock = new Mock<IUserService>();
        _validators = new Dictionary<Type, IValidator>();
        _controller = new UsersController(
            _loggerMock.Object,
            _validators,
            _serviceMock.Object
        );
    }

    private void MockHttpContextUserId(Guid? userId = null)
    {
        var id = userId ?? Guid.NewGuid();
        var httpContext = new DefaultHttpContext();
        httpContext.Items["UserId"] = id;
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, id.ToString())
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithApiResult()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<UserDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<IEnumerable<UserDto>>>(okResult.Value);
        Assert.True(apiResult.Success);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithApiResult()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(new UserDto { Id = id });

        var result = await _controller.GetById(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<UserDto>>(okResult.Value);
        Assert.True(apiResult.Success);
    }

    [Fact]
    public async Task Create_ReturnsOkResult_WithApiResult()
    {
        var dto = new CreateUserDto { FirstName = "Test" };
        _serviceMock.Setup(s => s.AddAsync(dto, It.IsAny<Guid>())).Returns(Task.CompletedTask);
        MockHttpContextUserId();

        var result = await _controller.Create(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<string>>(okResult.Value);
        Assert.True(apiResult.Success);
    }

    [Fact]
    public async Task Update_ReturnsOkResult_WithApiResult()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateUserDto { Id = id, FirstName = "Updated" };
        _serviceMock.Setup(s => s.UpdateAsync(dto, id, It.IsAny<Guid>())).Returns(Task.CompletedTask);
        MockHttpContextUserId();

        var result = await _controller.Update(id, dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<string>>(okResult.Value);
        Assert.True(apiResult.Success);
    }

    [Fact]
    public async Task Delete_ReturnsOkResult_WithApiResult()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteAsync(id, It.IsAny<Guid>())).Returns(Task.CompletedTask);
        MockHttpContextUserId();

        var result = await _controller.Delete(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<string>>(okResult.Value);
        Assert.True(apiResult.Success);
    }

    [Fact]
    public async Task GetPaged_ReturnsOkResult_WithApiResult()
    {
        _serviceMock.Setup(s => s.GetPagedAsync(1, 10)).ReturnsAsync(new PagedResult<UserDto>(new List<UserDto>(), 0, 1, 10));

        var result = await _controller.GetPaged(1, 10);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<PagedResult<UserDto>>>(okResult.Value);
        Assert.True(apiResult.Success);
    }
}