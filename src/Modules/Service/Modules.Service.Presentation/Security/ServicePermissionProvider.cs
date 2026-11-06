using Modules.Service.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Security;

public sealed class ServicePermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        ServicePermissions.List,
        ServicePermissions.Details,
        ServicePermissions.Create,
        ServicePermissions.Update,
        ServicePermissions.Delete,
        ServicePermissions.Categories,
        ServicePermissions.Materials,
        ServicePermissions.PersonnelAssignments,
        ServicePermissions.Tariffs
    ];
}
