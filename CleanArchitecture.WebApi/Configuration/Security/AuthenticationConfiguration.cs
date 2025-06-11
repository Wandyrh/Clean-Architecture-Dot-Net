using CleanArchitecture.WebApi.Common.Models;
using CleanArchitecture.WebApi.Configuration.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CleanArchitecture.WebApi.Configuration.Security;

public static class AuthenticationConfiguration
{
    public static void AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {        
        services.Configure<JWTOptions>(configuration.GetSection("JWTOptions"));
        services.PostConfigure<JWTOptions>(opts =>
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(opts.Audience);
            ArgumentException.ThrowIfNullOrWhiteSpace(opts.Issuer);
            ArgumentException.ThrowIfNullOrWhiteSpace(opts.Secret);
        });
       
        var jwtOptions = configuration
            .GetSection("JWTOptions")
            .Get<JWTOptions>() ?? throw new InvalidOperationException("JWTOptions section is missing.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtOptions.Issuer,
                ValidAudience = jwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret))
            };
        });
    }
}

