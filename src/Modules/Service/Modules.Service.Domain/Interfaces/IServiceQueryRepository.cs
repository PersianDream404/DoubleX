using Modules.Service.Domain.ReadModels;

namespace Modules.Service.Domain.Interfaces;

public interface IServiceQueryRepository
{
    Task<bool> ServiceOwnedAsync(long salonId, long serviceId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceReadModel>> GetServicesAsync(long salonId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceReadModel>> GetActiveServicesAsync(long salonId, CancellationToken cancellationToken);
}
