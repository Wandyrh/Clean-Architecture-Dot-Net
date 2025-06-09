using Asp.Versioning;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.WebApi.Common.Models;
using CleanArchitecture.WebApi.Controllers.Base;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.WebApi.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v1/[controller]")]
public class AuthenticationController : ApiControllerBase<AuthenticationController>
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(ILogger<AuthenticationController> logger,
        Dictionary<Type, IValidator> validators,
        IAuthenticationService authenticationService
        ) : base(logger, validators)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        await ValidateRequest(login);
        var loginResponse = await _authenticationService.AuthenticateUser(login);
        return Ok(ApiResult<LoginUserResponseDTO>.SuccessResult(loginResponse));
    }
}
