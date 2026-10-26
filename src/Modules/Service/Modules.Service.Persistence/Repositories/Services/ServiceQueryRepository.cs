using Microsoft.EntityFrameworkCore;
using Modules.Service.Domain.Interfaces;
using Modules.Service.Domain.ReadModels;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Mapper.Services;

namespace Modules.Service.Persistence.Repositories.Services;

public sealed class ServiceQueryRepository(ServiceReadDbContext readContext)
    : IServiceQueryRepository
{
    public Task<bool> ServiceOwnedAsync(
        long salonId,
        long serviceId,
        CancellationToken cancellationToken) =>
        readContext.Services.AnyAsync(
            service => service.Id == serviceId && service.SalonId == salonId,
            cancellationToken);

    public async Task<IReadOnlyList<ServiceReadModel>> GetServicesAsync(
        long salonId,
        CancellationToken cancellationToken) =>
        await readContext.Services
            .AsNoTracking()
            .Where(service => service.SalonId == salonId)
            .OrderBy(service => service.Name)
            .Select(ServiceMapper.ToReadModel())
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ServiceReadModel>> GetActiveServicesAsync(
        long salonId,
        CancellationToken cancellationToken) =>
        await readContext.Services
            .AsNoTracking()
            .Where(service => service.SalonId == salonId &&
                              service.Status &&
                              service.Category.Status)
            .OrderBy(service => service.Name)
            .Select(ServiceMapper.ToReadModel())
            .ToListAsync(cancellationToken);

    public Task<ServiceReadModel?> GetServiceByIdAsync(
        long id,
        CancellationToken cancellationToken) =>
        readContext.Services
            .AsNoTracking()
            .Where(service => service.Id == id)
            .Select(ServiceMapper.ToReadModel())
            .SingleOrDefaultAsync(cancellationToken);
}
