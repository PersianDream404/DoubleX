using Modules.Appointment.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.Appointment.Presentation.Security;

public sealed class AppointmentPermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        AppointmentPermissions.Create,
        AppointmentPermissions.List,
        AppointmentPermissions.Details,
        AppointmentPermissions.Update,
        AppointmentPermissions.Cancel
    ];
}
