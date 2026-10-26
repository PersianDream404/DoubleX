using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.Contract.Interfaces.Services;

public interface IServiceRepository
{
    Task<bool> ServiceOwnedAsync(long salonId, long serviceId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceDto>> GetServicesAsync(long salonId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceDto>> GetActiveServicesAsync(long salonId, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> CreateServiceAsync(ServiceRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> UpdateServiceAsync(long id, UpdateServiceRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> DeactivateServiceAsync(long salonId, long id, CancellationToken cancellationToken);
}
