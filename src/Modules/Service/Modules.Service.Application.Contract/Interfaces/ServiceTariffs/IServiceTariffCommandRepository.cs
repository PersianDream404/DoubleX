using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.Enums;

namespace Modules.Service.Application.Contract.Interfaces.ServiceTariffs;

public interface IServiceTariffCommandRepository
{
    Task<ServiceOperationStatus> CreateTariffAsync(ServiceTariffRequestDto request, CancellationToken cancellationToken);
    Task<ServiceOperationStatus> AddServicesAsync(long tariffId, AddServiceTariffItemsRequestDto request, CancellationToken cancellationToken);
}
