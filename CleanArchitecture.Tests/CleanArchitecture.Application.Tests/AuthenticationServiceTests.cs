using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Interfaces;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.Common.Exceptions;

namespace CleanArchitecture.Application.Tests;

public class AuthenticationServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly JWTOptions _jwtOptions = new() { Secret = "testsecretkeytestsecretkey", Issuer = "issuer", Audience = "aud", ExpiryHours = 1 };

    [Fact]
    public async Task AuthenticateUser_InvalidUser_ThrowsInvalidLoginException()
    {
        _unitOfWork.Setup(u => u.Users.GetUserByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null!);
        var service = new AuthenticationService(_unitOfWork.Object, _jwtOptions);

        await Assert.ThrowsAsync<InvalidLoginException>(() => service.AuthenticateUser(new LoginDto { UserName = "test", Password = "pass" }));
    }
}