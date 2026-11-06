using Identity.Application.Contract.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;

namespace Identity.Presentation.Endpoints.Permissions.Read;

public sealed class GetUserPermissionsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{UserPermissionApiInfo.Prefix}/users/{{userId:long}}/permissions", async (
                    long userId,
                    HttpContext context,
                    IUserPermissionManagementService service,
                    CancellationToken cancellationToken) =>
                {
                    if (!UserPermissionEndpointClaims.TryGetCaller(
                            context, out var actorUserId, out var salonId))
                        return Results.Unauthorized();

                    var result = await service.GetUserPermissionsAsync(
                        actorUserId, userId, salonId, cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : NotFound("کاربر موردنظر در سالن پیدا نشد.");
                })
                .RequireAuthorization(IdentityPermissions.ManageUserPermissions)
                .WithTags(UserPermissionApiInfo.Tag);

    }
}
