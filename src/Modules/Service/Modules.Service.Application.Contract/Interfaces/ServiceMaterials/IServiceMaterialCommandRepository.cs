using Modules.Service.Application.Contract.DTOs.ServiceMaterials;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.Contract.Interfaces.ServiceMaterials;

public interface IServiceMaterialCommandRepository
{
    Task<ServiceOperationStatus> UpsertMaterialAsync(long? id, ServiceMaterialRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> RemoveMaterialAsync(long salonId, long id, CancellationToken cancellationToken);
}
