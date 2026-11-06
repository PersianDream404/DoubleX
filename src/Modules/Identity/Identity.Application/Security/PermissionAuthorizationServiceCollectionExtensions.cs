using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Identity.Domain.Security;
using SharedKernel.Security;

namespace Identity.Application.Security;

public static class PermissionAuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        services
            .AddOptions<AuthorizationOptions>()
            .Configure<PermissionRegistry>((options, permissionRegistry) =>
            {
                RolePermissions.Validate(permissionRegistry);
                foreach (var permission in permissionRegistry.All)
                {
                    options.AddPolicy(permission, policy =>
                        policy
                            .RequireAuthenticatedUser()
                            .RequireClaim(PermissionClaimTypes.Permission, permission));
                }
            });

        return services;
    }
}
