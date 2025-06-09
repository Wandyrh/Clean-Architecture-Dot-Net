using CleanArchitecture.Application.DTOs;

namespace CleanArchitecture.Application.Interfaces;

public interface IAuthenticationService
{
    Task<LoginUserResponseDTO> AuthenticateUser(LoginDto login);
}