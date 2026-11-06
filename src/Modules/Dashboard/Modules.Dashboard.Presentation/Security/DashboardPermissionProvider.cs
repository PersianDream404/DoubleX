using Modules.Dashboard.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.Dashboard.Presentation.Security;

public sealed class DashboardPermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() => [DashboardPermissions.View];
}
