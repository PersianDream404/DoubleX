using Modules.Service.Domain.ReadModels;

namespace Modules.Service.Domain.Interfaces;

public interface IServiceMaterialQueryRepository
{
    Task<IReadOnlyList<ServiceMaterialReadModel>> GetMaterialsAsync(
        long salonId,
        long serviceId,
        CancellationToken cancellationToken);
}
