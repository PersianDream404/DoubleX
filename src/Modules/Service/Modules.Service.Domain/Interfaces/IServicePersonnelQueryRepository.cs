using Modules.Service.Domain.ReadModels;

namespace Modules.Service.Domain.Interfaces;

public interface IServicePersonnelQueryRepository
{
    Task<IReadOnlyList<ServicePersonnelReadModel>> GetByPersonnelAsync(
        long salonId,
        long personnelId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ServicePersonnelReadModel>> GetByServiceAsync(
        long salonId,
        long serviceId,
        CancellationToken cancellationToken);

    Task<ServicePersonnelReadModel?> GetByIdAsync(
        long salonId,
        long serviceId,
        long id,
        CancellationToken cancellationToken);
}
