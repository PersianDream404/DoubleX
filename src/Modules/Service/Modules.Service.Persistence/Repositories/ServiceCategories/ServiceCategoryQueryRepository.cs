using Microsoft.EntityFrameworkCore;
using Modules.Service.Domain.Interfaces;
using Modules.Service.Domain.ReadModels;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Repositories.ServiceCategories;

public sealed class ServiceCategoryQueryRepository(ServiceReadDbContext readContext)
    : IServiceCategoryQueryRepository
{
    public Task<bool> SalonExistsAsync(long salonId, CancellationToken cancellationToken) =>
        readContext.Set<ServiceSalonReference>().AnyAsync(salon => salon.Id == salonId, cancellationToken);

    public async Task<IReadOnlyList<ServiceCategoryReadModel>> GetCategoriesAsync(
        long salonId,
        CancellationToken cancellationToken) =>
        await readContext.ServiceCategories
            .AsNoTracking()
            .Where(category => category.SalonId == salonId)
            .OrderBy(category => category.Name)
            .Select(category => new ServiceCategoryReadModel(
                category.Id,
                category.SalonId,
                category.Name,
                category.Description,
                category.Status))
            .ToListAsync(cancellationToken);
}
