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

public class ProductCategoriesControllerTests
{
    private readonly Mock<ILogger<ProductCategoriesController>> _loggerMock;
    private readonly Mock<IProductCategoryService> _serviceMock;
    private readonly Dictionary<Type, IValidator> _validators;
    private readonly ProductCategoriesController _controller;
    private Guid _currentUserId;

    public ProductCategoriesControllerTests()
    {
        _loggerMock = new Mock<ILogger<ProductCategoriesController>>();
        _serviceMock = new Mock<IProductCategoryService>();
        _validators = new Dictionary<Type, IValidator>();
        _controller = new ProductCategoriesController(
            _loggerMock.Object,
            _validators,
            _serviceMock.Object
        );
        _currentUserId = Guid.NewGuid();
    }

    private void MockHttpContextUserId(Guid? userId = null)
    {
        var id = userId ?? _currentUserId;
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
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ProductCategoryDto>());

        var result = await _controller.GetAll();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<IEnumerable<ProductCategoryDto>>>(okResult.Value);
        Assert.True(apiResult.Success);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithApiResult()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(new ProductCategoryDto { Id = id });

        var result = await _controller.GetById(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<ProductCategoryDto>>(okResult.Value);
        Assert.True(apiResult.Success);
    }

    [Fact]
    public async Task Create_ReturnsOkResult_WithApiResult()
    {
        var dto = new CreateProductCategoryDto { Name = "Test" };
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
        var dto = new UpdateProductCategoryDto { Id = id, Name = "Updated" };
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
        _serviceMock.Setup(s => s.GetPagedAsync(1, 10)).ReturnsAsync(new PagedResult<ProductCategoryDto>(new List<ProductCategoryDto>(), 0, 1, 10));

        var result = await _controller.GetPaged(1, 10);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<PagedResult<ProductCategoryDto>>>(okResult.Value);
        Assert.True(apiResult.Success);
    }
}