using Microsoft.EntityFrameworkCore;
using Modules.Service.Application.Contract.DTOs.ServiceMaterials;
using Modules.Service.Application.Contract.Enums;
using Modules.Service.Application.Contract.Interfaces.ServiceMaterials;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Mapper.ServiceMaterials;

namespace Modules.Service.Persistence.Repositories.ServiceMaterials;

public sealed class ServiceMaterialRepository(
    ServiceWriteDbContext writeContext,
    ServiceReadDbContext readContext) : IServiceMaterialRepository
{
    public async Task<IReadOnlyList<ServiceMaterialDto>> GetMaterialsAsync(
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
            .Select(ServiceMaterialMapper.ToDto())
            .ToListAsync(cancellationToken);

    public async Task<ServiceOperationStatus> UpsertMaterialAsync(
        long? id,
        ServiceMaterialRequestDto request,
        CancellationToken cancellationToken)
    {
        var service = await writeContext.Services.SingleOrDefaultAsync(
            item => item.Id == request.ServiceId && item.SalonId == request.SalonId,
            cancellationToken);
        if (service is null)
            return ServiceOperationStatus.NotFound;
        if (!service.Status)
            return ServiceOperationStatus.InvalidOwnership;

        var material = await writeContext.Set<ServiceInventoryMaterialReference>()
            .SingleOrDefaultAsync(item => item.Id == request.MaterialId, cancellationToken);
        if (material is null)
            return ServiceOperationStatus.NotFound;
        if (!material.IsActive)
            return ServiceOperationStatus.InactiveMaterial;

        ServiceMaterial? existing;
        if (id.HasValue)
        {
            existing = await writeContext.ServiceMaterials.SingleOrDefaultAsync(
                item => item.Id == id.Value && item.SalonId == request.SalonId,
                cancellationToken);
            if (existing is null)
                return ServiceOperationStatus.NotFound;
            if (await writeContext.ServiceMaterials.AnyAsync(
                    item => item.Id != id.Value &&
                            item.ServiceId == request.ServiceId &&
                            item.MaterialId == request.MaterialId,
                    cancellationToken))
                return ServiceOperationStatus.Conflict;
            existing.Update(request.QuantityPerService, request.Description);
        }
        else
        {
            existing = await writeContext.ServiceMaterials.SingleOrDefaultAsync(
                item => item.SalonId == request.SalonId &&
                        item.ServiceId == request.ServiceId &&
                        item.MaterialId == request.MaterialId,
                cancellationToken);
            if (existing is not null)
                existing.Update(request.QuantityPerService, request.Description);
            else
                await writeContext.ServiceMaterials.AddAsync(
                    ServiceMaterial.Create(
                        request.SalonId,
                        request.ServiceId,
                        request.MaterialId,
                        request.QuantityPerService,
                        request.Description),
                    cancellationToken);
        }

        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> RemoveMaterialAsync(
        long salonId,
        long id,
        CancellationToken cancellationToken)
    {
        var material = await writeContext.ServiceMaterials.SingleOrDefaultAsync(
            item => item.Id == id && item.SalonId == salonId,
            cancellationToken);
        if (material is null)
            return ServiceOperationStatus.NotFound;
        material.Deleted = true;
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }
}
