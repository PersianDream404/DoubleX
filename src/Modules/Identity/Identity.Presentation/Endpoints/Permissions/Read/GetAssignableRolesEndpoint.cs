using Identity.Application.Contract.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel.Interface;

namespace Identity.Presentation.Endpoints.Permissions.Read;

public sealed class GetAssignableRolesEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{UserPermissionApiInfo.Prefix}/roles/assignable", (
                    IUserPermissionManagementService service) =>
                Ok(service.GetAssignableRoles()))
                .RequireAuthorization(IdentityPermissions.ManageUserPermissions)
                .WithTags(UserPermissionApiInfo.Tag);
    }
}
