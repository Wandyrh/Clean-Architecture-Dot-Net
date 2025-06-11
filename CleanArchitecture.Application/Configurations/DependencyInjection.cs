using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Configurations;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration
            .GetSection("JWTOptions")
            .Get<JWTOptions>() ?? throw new InvalidOperationException("JWTOptions is not configured.");
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtOptions.Audience);
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtOptions.Issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(jwtOptions.Secret);

        services.AddSingleton(jwtOptions);
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());        
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IProductCategoryService, ProductCategoryService>();

        return services;
    }
}
