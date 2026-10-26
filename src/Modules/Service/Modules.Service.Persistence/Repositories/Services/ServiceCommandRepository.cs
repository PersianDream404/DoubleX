using Microsoft.EntityFrameworkCore;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.Enums;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;
using ServiceEntity = Modules.Service.Domain.Entities.Service;

namespace Modules.Service.Persistence.Repositories.Services;

public sealed class ServiceCommandRepository(ServiceWriteDbContext writeContext)
    : IServiceCommandRepository
{
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
            var salonExists = await writeContext.Set<ServiceSalonReference>()
                .AnyAsync(salon => salon.Id == request.SalonId, cancellationToken);
            return salonExists ? ServiceOperationStatus.InvalidOwnership : ServiceOperationStatus.NotFound;
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
