using Identity.Application.Contract.Security;
using Identity.Presentation.Endpoints.Permissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;

namespace Identity.Presentation.Endpoints.Permissions.Read;

public sealed class GetAvailablePermissionsEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{UserPermissionApiInfo.Prefix}/permissions", (
                    IUserPermissionManagementService service) =>
                Ok(service.GetAvailablePermissions()))
                .RequireAuthorization(IdentityPermissions.ManageUserPermissions)
                .WithTags(UserPermissionApiInfo.Tag);
    }
}
