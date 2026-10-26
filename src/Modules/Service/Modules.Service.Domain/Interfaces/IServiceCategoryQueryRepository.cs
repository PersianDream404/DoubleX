using Modules.Service.Domain.ReadModels;

namespace Modules.Service.Domain.Interfaces;

public interface IServiceCategoryQueryRepository
{
    Task<bool> SalonExistsAsync(long salonId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ServiceCategoryReadModel>> GetCategoriesAsync(
        long salonId,
        CancellationToken cancellationToken);
    Task<ServiceCategoryReadModel?> GetCategoryByIdAsync(long id, CancellationToken cancellationToken);
}
