using Microsoft.EntityFrameworkCore;
using Modules.Service.Domain.Interfaces;
using Modules.Service.Domain.ReadModels;
using Modules.Service.Persistence.Context;

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
            .Select(service => new ServiceReadModel(
                service.Id,
                service.SalonId,
                service.CategoryId,
                service.Code,
                service.Name,
                service.Description,
                service.DurationMinutes,
                service.Status))
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
            .Select(service => new ServiceReadModel(
                service.Id,
                service.SalonId,
                service.CategoryId,
                service.Code,
                service.Name,
                service.Description,
                service.DurationMinutes,
                service.Status))
            .ToListAsync(cancellationToken);
}
