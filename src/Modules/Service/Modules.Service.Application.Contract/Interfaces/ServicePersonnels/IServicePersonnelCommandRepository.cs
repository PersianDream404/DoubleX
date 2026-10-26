using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.Contract.Interfaces.ServicePersonnels;

public interface IServicePersonnelCommandRepository
{
    Task<ServiceOperationStatus> UpsertPersonnelAsync(
        long? id,
        ServicePersonnelRequestDto request,
        CancellationToken cancellationToken);

    Task<ServiceOperationStatus> RemovePersonnelAsync(
        long salonId,
        long serviceId,
        long id,
        CancellationToken cancellationToken);
}
