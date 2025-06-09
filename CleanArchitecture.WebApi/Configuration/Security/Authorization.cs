using CleanArchitecture.WebApi.Common.Models;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitecture.WebApi.Configuration.Security;

public static class Authorization
{
    public static void ConfigureScopes(this IServiceCollection services, SecuritySettings securitySettings)
    {
        services.AddAuthorization(options =>
        {
            string[] permissions =
            [
            ];

            foreach (var permission in permissions)
            {
                options.AddPolicy(permission, policy => policy.Requirements.Add(new HasScopeRequirement(permission, securitySettings.Issuer)));
            }
        });

        services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
    }
}
