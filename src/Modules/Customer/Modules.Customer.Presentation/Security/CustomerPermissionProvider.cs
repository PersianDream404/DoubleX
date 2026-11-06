using Modules.Customer.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation.Security;

public sealed class CustomerPermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        CustomerPermissions.Create,
        CustomerPermissions.List,
        CustomerPermissions.Details,
        CustomerPermissions.Update,
        CustomerPermissions.Delete,
        CustomerPermissions.Toggle
    ];
}
