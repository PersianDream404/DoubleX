using Modules.Service.Application.Contract.DTOs.ServicePrices;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.Contract.Interfaces.ServicePrices;

public interface IServicePriceRepository
{
    Task<IReadOnlyList<ServicePriceDto>> GetPriceHistoryAsync(long salonId, long serviceId, CancellationToken cancellationToken);
    Task<ServicePriceDto?> GetPriceAtAsync(long salonId, long serviceId, DateTimeOffset at, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> AddPriceAsync(CreateServicePriceRequestDto request, CancellationToken cancellationToken);
}
