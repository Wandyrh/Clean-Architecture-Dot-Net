using CleanArchitecture.Application.Common.Exceptions;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Models;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Domain.Interfaces;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitecture.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUnitOfWork _unitOfWork; 
    private readonly JWTOptions _jwtOptions;    
    public AuthenticationService(IUnitOfWork unitOfWork, JWTOptions jwtOptions)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));     
        _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
    }

    public async Task<LoginUserResponseDTO> AuthenticateUser(LoginDto login)
    {
        var user = await _unitOfWork.Users.GetUserByEmailAsync(login.UserName);
        if (user is null)
            throw new InvalidLoginException("Invalid credentials.");

        var hash = HashHelper.GenerateSha256Hash(login.Password);
        if (user.Password == hash)
        { 
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtOptions.Secret);
            var expiresAt = DateTime.UtcNow.AddHours(_jwtOptions.ExpiryHours > 0 ? _jwtOptions.ExpiryHours : 1);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email), 
                new Claim("FullName", $"{user.FirstName} {user.LastName}")
            };           

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return new LoginUserResponseDTO()
            {
                AccessToken = tokenString,
                ExpiresAt = expiresAt,
            };
        }

        throw new InvalidLoginException("Invalid credentials.");
    }
}
