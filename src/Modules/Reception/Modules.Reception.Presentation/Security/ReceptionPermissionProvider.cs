using Modules.Reception.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.Reception.Presentation.Security;

public sealed class ReceptionPermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        ReceptionPermissions.Create,
        ReceptionPermissions.List,
        ReceptionPermissions.Details,
        ReceptionPermissions.Update,
        ReceptionPermissions.Cancel,
        ReceptionPermissions.Reports
    ];
}
