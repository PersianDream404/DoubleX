using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.Contract.Interfaces.Services;

public interface IServiceCommandRepository
{
    Task<ServiceOperationStatus> CreateServiceAsync(ServiceRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> UpdateServiceAsync(long id, UpdateServiceRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> DeactivateServiceAsync(long salonId, long id, CancellationToken cancellationToken);
}
