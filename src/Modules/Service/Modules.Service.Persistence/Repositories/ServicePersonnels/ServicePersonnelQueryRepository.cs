using Microsoft.EntityFrameworkCore;
using Modules.Service.Domain.Interfaces;
using Modules.Service.Domain.ReadModels;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Mapper.ServicePersonnels;

namespace Modules.Service.Persistence.Repositories.ServicePersonnels;

public sealed class ServicePersonnelQueryRepository(ServiceReadDbContext readContext)
    : IServicePersonnelQueryRepository
{
    public async Task<IReadOnlyList<ServicePersonnelReadModel>> GetByPersonnelAsync(
        long salonId,
        long personnelId,
        CancellationToken cancellationToken) =>
        await readContext.ServicePersonnels
            .AsNoTracking()
            .Where(item => item.SalonId == salonId && item.PersonnelId == personnelId)
            .OrderBy(item => item.ServiceId)
            .Select(ServicePersonnelMapper.ToReadModel())
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ServicePersonnelReadModel>> GetByServiceAsync(
        long salonId,
        long serviceId,
        CancellationToken cancellationToken) =>
        await readContext.ServicePersonnels
            .AsNoTracking()
            .Where(item => item.SalonId == salonId && item.ServiceId == serviceId)
            .OrderBy(item => item.PersonnelId)
            .Select(ServicePersonnelMapper.ToReadModel())
            .ToListAsync(cancellationToken);

    public Task<ServicePersonnelReadModel?> GetByIdAsync(
        long salonId,
        long serviceId,
        long id,
        CancellationToken cancellationToken) =>
        readContext.ServicePersonnels
            .AsNoTracking()
            .Where(item => item.Id == id &&
                           item.SalonId == salonId &&
                           item.ServiceId == serviceId)
            .Select(ServicePersonnelMapper.ToReadModel())
            .SingleOrDefaultAsync(cancellationToken);
}
