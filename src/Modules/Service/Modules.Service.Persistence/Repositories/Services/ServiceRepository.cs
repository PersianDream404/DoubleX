using Microsoft.EntityFrameworkCore;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.Enums;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Mapper.Services;
using ServiceEntity = Modules.Service.Domain.Entities.Service;

namespace Modules.Service.Persistence.Repositories.Services;

public sealed class ServiceRepository(
    ServiceWriteDbContext writeContext,
    ServiceReadDbContext readContext,
    IServiceCategoryRepository categoryRepository) : IServiceRepository
{
    public Task<bool> ServiceOwnedAsync(
        long salonId,
        long serviceId,
        CancellationToken cancellationToken) =>
        readContext.Services.AnyAsync(
            service => service.Id == serviceId && service.SalonId == salonId,
            cancellationToken);

    public async Task<IReadOnlyList<ServiceDto>> GetServicesAsync(
        long salonId,
        CancellationToken cancellationToken) =>
        await readContext.Services
            .AsNoTracking()
            .Where(service => service.SalonId == salonId)
            .OrderBy(service => service.Name)
            .Select(ServiceMapper.ToDto())
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ServiceDto>> GetActiveServicesAsync(
        long salonId,
        CancellationToken cancellationToken) =>
        await readContext.Services
            .AsNoTracking()
            .Where(service => service.SalonId == salonId &&
                              service.Status &&
                              service.Category.Status)
            .OrderBy(service => service.Name)
            .Select(ServiceMapper.ToDto())
            .ToListAsync(cancellationToken);

    public async Task<ServiceOperationStatus> CreateServiceAsync(
        ServiceRequestDto request,
        CancellationToken cancellationToken)
    {
        var category = await writeContext.ServiceCategories.SingleOrDefaultAsync(
            item => item.Id == request.CategoryId &&
                    item.SalonId == request.SalonId &&
                    item.Status,
            cancellationToken);
        if (category is null)
        {
            return await categoryRepository.SalonExistsAsync(request.SalonId, cancellationToken)
                ? ServiceOperationStatus.InvalidOwnership
                : ServiceOperationStatus.NotFound;
        }
        if (await writeContext.Services.AnyAsync(
                item => item.SalonId == request.SalonId &&
                        item.Code == request.Code.Trim(),
                cancellationToken))
            return ServiceOperationStatus.Conflict;

        await writeContext.Services.AddAsync(
            ServiceEntity.Create(
                request.SalonId,
                request.CategoryId,
                request.Code,
                request.Name,
                request.Description,
                request.DurationMinutes),
            cancellationToken);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> UpdateServiceAsync(
        long id,
        UpdateServiceRequestDto request,
        CancellationToken cancellationToken)
    {
        var service = await writeContext.Services.SingleOrDefaultAsync(
            item => item.Id == id && item.SalonId == request.SalonId,
            cancellationToken);
        if (service is null)
            return ServiceOperationStatus.NotFound;

        var categoryExists = await writeContext.ServiceCategories.AnyAsync(
            item => item.Id == request.CategoryId &&
                    item.SalonId == request.SalonId &&
                    item.Status,
            cancellationToken);
        if (!categoryExists)
            return ServiceOperationStatus.InvalidOwnership;
        if (await writeContext.Services.AnyAsync(
                item => item.SalonId == request.SalonId &&
                        item.Id != id &&
                        item.Code == request.Code.Trim(),
                cancellationToken))
            return ServiceOperationStatus.Conflict;

        service.Update(
            request.CategoryId,
            request.Code,
            request.Name,
            request.Description,
            request.DurationMinutes,
            request.IsActive);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> DeactivateServiceAsync(
        long salonId,
        long id,
        CancellationToken cancellationToken)
    {
        var service = await writeContext.Services.SingleOrDefaultAsync(
            item => item.Id == id && item.SalonId == salonId,
            cancellationToken);
        if (service is null)
            return ServiceOperationStatus.NotFound;
        service.Deactivate();
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }
}
