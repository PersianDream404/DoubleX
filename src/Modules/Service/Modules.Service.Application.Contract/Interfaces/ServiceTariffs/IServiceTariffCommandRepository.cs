using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.Contract.Interfaces.ServiceTariffs;

public interface IServiceTariffCommandRepository
{
    Task<ServiceOperationStatus> CreateTariffAsync(ServiceTariffRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> UpdateTariffAsync(long id, ServiceTariffRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> DeleteTariffAsync(long id, long salonId, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> AddServicesAsync(long tariffId, AddServiceTariffItemsRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> UpdateServiceAmountAsync(long tariffId, long serviceId, UpdateServiceTariffItemRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> RemoveServiceAsync(long tariffId, long serviceId, long salonId, CancellationToken cancellationToken);
}
