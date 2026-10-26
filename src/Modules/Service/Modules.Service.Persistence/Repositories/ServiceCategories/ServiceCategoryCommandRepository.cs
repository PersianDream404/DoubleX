using Microsoft.EntityFrameworkCore;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.Enums;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Repositories.ServiceCategories;

public sealed class ServiceCategoryCommandRepository(ServiceWriteDbContext writeContext)
    : IServiceCategoryCommandRepository
{
    public async Task<ServiceOperationStatus> CreateCategoryAsync(
        ServiceCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!await writeContext.Set<ServiceSalonReference>()
                .AnyAsync(salon => salon.Id == request.SalonId, cancellationToken))
            return ServiceOperationStatus.NotFound;
        var existingCategory = await writeContext.ServiceCategories
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(
                category => category.SalonId == request.SalonId &&
                            category.Name == request.Name.Trim(),
                cancellationToken);
        if (existingCategory is not null)
        {
            if (!existingCategory.Deleted)
                return ServiceOperationStatus.Conflict;

            existingCategory.Deleted = false;
            existingCategory.Update(request.Name, request.Description, true);
            await writeContext.SaveChangesAsync(cancellationToken);
            return ServiceOperationStatus.Succeeded;
        }

        await writeContext.ServiceCategories.AddAsync(
            ServiceCategory.Create(request.SalonId, request.Name, request.Description),
            cancellationToken);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> UpdateCategoryAsync(
        long id,
        UpdateServiceCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        var category = await writeContext.ServiceCategories.SingleOrDefaultAsync(
            item => item.Id == id && item.SalonId == request.SalonId,
            cancellationToken);
        if (category is null)
            return ServiceOperationStatus.NotFound;
        if (await writeContext.ServiceCategories.AnyAsync(
                item => item.SalonId == request.SalonId &&
                        item.Id != id &&
                        item.Name == request.Name.Trim(),
                cancellationToken))
            return ServiceOperationStatus.Conflict;

        category.Update(request.Name, request.Description, request.IsActive);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> DeactivateCategoryAsync(
        long salonId,
        long id,
        CancellationToken cancellationToken)
    {
        var category = await writeContext.ServiceCategories.SingleOrDefaultAsync(
            item => item.Id == id && item.SalonId == salonId,
            cancellationToken);
        if (category is null)
            return ServiceOperationStatus.NotFound;
        category.Deactivate();
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> DeleteCategoryAsync(
        long salonId,
        long id,
        CancellationToken cancellationToken)
    {
        var category = await writeContext.ServiceCategories.SingleOrDefaultAsync(
            item => item.Id == id && item.SalonId == salonId,
            cancellationToken);
        if (category is null)
            return ServiceOperationStatus.NotFound;

        if (await writeContext.Services.IgnoreQueryFilters().AnyAsync(
                service => service.SalonId == salonId && service.CategoryId == id,
                cancellationToken))
            return ServiceOperationStatus.Conflict;

        category.Deleted = true;
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }
}
