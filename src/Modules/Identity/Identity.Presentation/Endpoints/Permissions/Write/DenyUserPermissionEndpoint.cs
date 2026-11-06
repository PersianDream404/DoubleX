using Identity.Application.Contract.Security;
using Identity.Application.Services;
using Identity.Domain.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;

namespace Identity.Presentation.Endpoints.Permissions.Write;

public sealed class DenyUserPermissionEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut(
                    $"{UserPermissionApiInfo.Prefix}/users/{{userId:long}}/permissions/{{permission}}/deny",
                    async (
                        long userId,
                        string permission,
                        HttpContext context,
                        IUserPermissionManagementService service,
                        CancellationToken cancellationToken) =>
                    {
                        if (!UserPermissionEndpointClaims.TryGetCaller(
                                context, out var actorUserId, out var salonId))
                            return Results.Unauthorized();

                        var result = await service.SetOverrideAsync(
                            actorUserId,
                            userId,
                            salonId,
                            permission,
                            UserPermissionEffect.Deny,
                            cancellationToken);
                        return result.IsSuccess
                            ? Results.NoContent()
                            : Results.NotFound();
                    })
                .RequireAuthorization(IdentityPermissions.ManageUserPermissions)
                .WithTags(UserPermissionApiInfo.Tag);
    }
}
