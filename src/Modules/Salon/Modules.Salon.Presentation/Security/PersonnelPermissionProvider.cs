using Modules.Personnel.Application.Contract.Security;
using Modules.Salon.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.Personnel.Presentation.Security;

public sealed class PersonnelPermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        PersonnelPermissions.Create,
        PersonnelPermissions.List,
        PersonnelPermissions.Details,
        PersonnelPermissions.Update,
        PersonnelPermissions.Delete,
        SalonPermissions.List,
        SalonPermissions.Details,
        SalonPermissions.Create,
        SalonPermissions.Update,
        SalonPermissions.Delete,
        SalonPermissions.Toggle
    ];
}
