using Microsoft.EntityFrameworkCore;
using Modules.Service.Domain.Interfaces;
using Modules.Service.Domain.ReadModels;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Mapper.ServiceMaterials;

namespace Modules.Service.Persistence.Repositories.ServiceMaterials;

public sealed class ServiceMaterialQueryRepository(ServiceReadDbContext readContext)
    : IServiceMaterialQueryRepository
{
    public async Task<IReadOnlyList<ServiceMaterialReadModel>> GetMaterialsAsync(
        long salonId,
        long serviceId,
        CancellationToken cancellationToken) =>
        await (
            from entry in readContext.ServiceMaterials.AsNoTracking()
            join material in readContext.Set<ServiceInventoryMaterialReference>()
                on entry.MaterialId equals material.Id
            join unit in readContext.Set<ServiceInventoryUnitReference>()
                on material.BaseUnitId equals unit.Id
            where entry.SalonId == salonId && entry.ServiceId == serviceId
            orderby material.Name
            select new ServiceMaterialProjection
            {
                Entry = entry,
                Material = material,
                Unit = unit
            })
            .Select(ServiceMaterialMapper.ToReadModel())
            .ToListAsync(cancellationToken);
}
