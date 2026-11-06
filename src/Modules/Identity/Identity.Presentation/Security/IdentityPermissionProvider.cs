using Identity.Application.Contract.Security;
using SharedKernel.Interface;

namespace Identity.Presentation.Security;

public sealed class IdentityPermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        IdentityPermissions.ManageUserPermissions
    ];
}
