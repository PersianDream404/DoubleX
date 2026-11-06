using Identity.Application.Contract.Security;
using Identity.Application.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;

namespace Identity.Presentation.Endpoints.Permissions.Write;

public sealed class ResetUserPermissionOverridesEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapDelete(
                    $"{UserPermissionApiInfo.Prefix}/users/{{userId:long}}/permissions",
                    async (
                        long userId,
                        HttpContext context,
                        IUserPermissionManagementService service,
                        CancellationToken cancellationToken) =>
                    {
                        if (!UserPermissionEndpointClaims.TryGetCaller(
                                context, out var actorUserId, out var salonId))
                            return Results.Unauthorized();

                        var result = await service.ResetOverridesAsync(
                            actorUserId, userId, salonId, cancellationToken);
                        return result.IsSuccess
                            ? Results.NoContent()
                            : Results.NotFound();
                    })
                .RequireAuthorization(IdentityPermissions.ManageUserPermissions)
                .WithTags(UserPermissionApiInfo.Tag);
    }
}
