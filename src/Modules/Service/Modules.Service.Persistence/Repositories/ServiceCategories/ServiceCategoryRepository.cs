using Microsoft.EntityFrameworkCore;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.Enums;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Mapper.ServiceCategories;

namespace Modules.Service.Persistence.Repositories.ServiceCategories;

public sealed class ServiceCategoryRepository(
    ServiceWriteDbContext writeContext,
    ServiceReadDbContext readContext) : IServiceCategoryRepository
{
    public Task<bool> SalonExistsAsync(long salonId, CancellationToken cancellationToken) =>
        readContext.Set<ServiceSalonReference>().AnyAsync(salon => salon.Id == salonId, cancellationToken);

    public async Task<IReadOnlyList<ServiceCategoryDto>> GetCategoriesAsync(
        long salonId,
        CancellationToken cancellationToken) =>
        await readContext.ServiceCategories
            .AsNoTracking()
            .Where(category => category.SalonId == salonId)
            .OrderBy(category => category.Name)
            .Select(ServiceCategoryMapper.ToDto())
            .ToListAsync(cancellationToken);

    public async Task<ServiceOperationStatus> CreateCategoryAsync(
        ServiceCategoryRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!await SalonExistsAsync(request.SalonId, cancellationToken))
            return ServiceOperationStatus.NotFound;
        if (await writeContext.ServiceCategories.AnyAsync(
                category => category.SalonId == request.SalonId &&
                            category.Name == request.Name.Trim(),
                cancellationToken))
            return ServiceOperationStatus.Conflict;

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
}
