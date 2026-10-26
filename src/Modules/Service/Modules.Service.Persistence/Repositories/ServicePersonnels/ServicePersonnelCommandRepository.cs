using Microsoft.EntityFrameworkCore;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.Enums;
using Modules.Service.Application.Contract.Interfaces.ServicePersonnels;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Repositories.ServicePersonnels;

public sealed class ServicePersonnelCommandRepository(ServiceWriteDbContext writeContext)
    : IServicePersonnelCommandRepository
{
    public async Task<ServiceOperationStatus> UpsertPersonnelAsync(
        long? id,
        ServicePersonnelRequestDto request,
        CancellationToken cancellationToken)
    {
        var service = await writeContext.Services.SingleOrDefaultAsync(
            item => item.Id == request.ServiceId && item.SalonId == request.SalonId,
            cancellationToken);
        if (service is null)
            return ServiceOperationStatus.NotFound;
        if (!service.Status)
            return ServiceOperationStatus.InvalidOwnership;

        ServicePersonnel? existing;
        if (id.HasValue)
        {
            existing = await writeContext.ServicePersonnels.SingleOrDefaultAsync(
                item => item.Id == id.Value &&
                        item.SalonId == request.SalonId &&
                        item.ServiceId == request.ServiceId,
                cancellationToken);
            if (existing is null)
                return ServiceOperationStatus.NotFound;
            if (await writeContext.ServicePersonnels.AnyAsync(
                    item => item.Id != id.Value &&
                            item.SalonId == request.SalonId &&
                            item.ServiceId == request.ServiceId &&
                            item.PersonnelId == request.PersonnelId,
                    cancellationToken))
            {
                return ServiceOperationStatus.Conflict;
            }

            existing.Update(request.ServiceId, request.PersonnelId);
        }
        else
        {
            existing = await writeContext.ServicePersonnels
                .IgnoreQueryFilters()
                .SingleOrDefaultAsync(
                    item => item.SalonId == request.SalonId &&
                            item.ServiceId == request.ServiceId &&
                            item.PersonnelId == request.PersonnelId,
                    cancellationToken);
            if (existing is not null)
            {
                if (!existing.Deleted)
                    return ServiceOperationStatus.Conflict;
                existing.Deleted = false;
                existing.Status = true;
            }
            else
            {
                await writeContext.ServicePersonnels.AddAsync(
                    ServicePersonnel.Create(
                        request.SalonId,
                        request.ServiceId,
                        request.PersonnelId),
                    cancellationToken);
            }
        }

        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> RemovePersonnelAsync(
        long salonId,
        long serviceId,
        long id,
        CancellationToken cancellationToken)
    {
        var item = await writeContext.ServicePersonnels.SingleOrDefaultAsync(
            entry => entry.Id == id &&
                     entry.SalonId == salonId &&
                     entry.ServiceId == serviceId,
            cancellationToken);
        if (item is null)
            return ServiceOperationStatus.NotFound;

        item.Deleted = true;
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }
}
