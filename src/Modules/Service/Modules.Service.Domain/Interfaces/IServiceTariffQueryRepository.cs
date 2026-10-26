using Modules.Service.Domain.ReadModels;

namespace Modules.Service.Domain.Interfaces;

public interface IServiceTariffQueryRepository
{
    Task<bool> SalonExistsAsync(long salonId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceTariffReadModel>> GetTariffsAsync(
        long salonId,
        CancellationToken cancellationToken);
    Task<ServiceTariffReadModel?> GetTariffByIdAsync(long id, CancellationToken cancellationToken);
}
