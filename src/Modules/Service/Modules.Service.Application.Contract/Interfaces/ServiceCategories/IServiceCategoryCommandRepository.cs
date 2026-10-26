using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.Contract.Interfaces.ServiceCategories;

public interface IServiceCategoryCommandRepository
{
    Task<ServiceOperationStatus> CreateCategoryAsync(ServiceCategoryRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> UpdateCategoryAsync(long id, UpdateServiceCategoryRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> DeactivateCategoryAsync(long salonId, long id, CancellationToken cancellationToken);
}
