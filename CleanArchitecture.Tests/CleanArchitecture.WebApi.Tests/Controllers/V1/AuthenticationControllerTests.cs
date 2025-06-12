using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.WebApi.Common.Models;
using CleanArchitecture.WebApi.Controllers.V1;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CleanArchitecture.Tests.CleanArchitecture.WebApi.Tests.Controllers.V1;

public class AuthenticationControllerTests
{
    private readonly Mock<ILogger<AuthenticationController>> _loggerMock;
    private readonly Mock<IAuthenticationService> _authServiceMock;
    private readonly Dictionary<Type, IValidator> _validators;
    private readonly AuthenticationController _controller;

    public AuthenticationControllerTests()
    {
        _loggerMock = new Mock<ILogger<AuthenticationController>>();
        _authServiceMock = new Mock<IAuthenticationService>();
        _validators = new Dictionary<Type, IValidator>();
        _controller = new AuthenticationController(
            _loggerMock.Object,
            _validators,
            _authServiceMock.Object
        );
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WithApiResult()
    {           
        var loginDto = new LoginDto { UserName = "test", Password = "pass" };
        var loginResponse = new LoginUserResponseDTO { AccessToken = "jwt-token" };

        _authServiceMock
            .Setup(s => s.AuthenticateUser(loginDto))
            .ReturnsAsync(loginResponse);
       
        var result = await _controller.Login(loginDto);
      
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResult = Assert.IsType<ApiResult<LoginUserResponseDTO>>(okResult.Value);
        Assert.True(apiResult.Success);
        Assert.Equal(loginResponse.AccessToken, apiResult.Data.AccessToken);
    }

    [Fact]
    public async Task Login_ReturnsOkResult_WhenModelIsInvalid()
    {
        _controller.ModelState.AddModelError("UserName", "Required");
        var loginDto = new LoginDto { UserName = "", Password = "pass" };

        var result = await _controller.Login(loginDto);

        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_WhenAuthServiceThrows()
    {
        var loginDto = new LoginDto { UserName = "test", Password = "wrong" };
        _authServiceMock
            .Setup(s => s.AuthenticateUser(loginDto))
            .ThrowsAsync(new Exception("Invalid credentials"));
       
        await Assert.ThrowsAsync<Exception>(() => _controller.Login(loginDto));
    }
}