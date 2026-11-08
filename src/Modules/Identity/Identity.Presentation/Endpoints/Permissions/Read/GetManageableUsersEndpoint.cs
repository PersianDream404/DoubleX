using Identity.Application.Contract.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;

namespace Identity.Presentation.Endpoints.Permissions.Read;

public sealed class GetManageableUsersEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{UserPermissionApiInfo.Prefix}/users/assignable", async (
                    HttpContext context,
                    IUserPermissionManagementService service,
                    CancellationToken cancellationToken) =>
                {
                    if (!UserPermissionEndpointClaims.TryGetCaller(
                            context, out var actorUserId, out var salonId))
                    {
                        return Results.Unauthorized();
                    }

                    var result = await service.GetManageableUsersAsync(
                        actorUserId, salonId, cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : NotFound("کاربر قابل مدیریتی برای این حساب پیدا نشد.");
                })
                .RequireAuthorization(IdentityPermissions.ManageUserPermissions)
                .WithTags(UserPermissionApiInfo.Tag);
    }
}
