using Microsoft.EntityFrameworkCore;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.Enums;
using Modules.Service.Application.Contract.Interfaces.ServiceTariffs;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Repositories.ServiceTariffs;

public sealed class ServiceTariffCommandRepository(ServiceWriteDbContext writeContext)
    : IServiceTariffCommandRepository
{
    public async Task<ServiceOperationStatus> CreateTariffAsync(
        ServiceTariffRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!await writeContext.Set<ServiceSalonReference>()
                .AnyAsync(salon => salon.Id == request.SalonId, cancellationToken))
            return ServiceOperationStatus.NotFound;
        if (request.ValidTo.HasValue && request.ValidTo.Value <= request.ValidFrom)
            return ServiceOperationStatus.InvalidTariffValidity;
        if (await writeContext.ServiceTariffs.AnyAsync(
                tariff => tariff.SalonId == request.SalonId && tariff.Name == request.Name.Trim(),
                cancellationToken))
            return ServiceOperationStatus.Conflict;

        await writeContext.ServiceTariffs.AddAsync(
            ServiceTariff.Create(request.SalonId, request.Name, request.ValidFrom, request.ValidTo),
            cancellationToken);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> AddServicesAsync(
        long tariffId,
        AddServiceTariffItemsRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!await writeContext.Set<ServiceSalonReference>()
                .AnyAsync(salon => salon.Id == request.SalonId, cancellationToken))
            return ServiceOperationStatus.NotFound;

        var tariff = await writeContext.ServiceTariffs.SingleOrDefaultAsync(
            item => item.Id == tariffId && item.SalonId == request.SalonId,
            cancellationToken);
        if (tariff is null)
            return ServiceOperationStatus.NotFound;

        var serviceIds = request.Services.Select(service => service.ServiceId).ToArray();
        if (serviceIds.Distinct().Count() != serviceIds.Length ||
            await writeContext.Services.CountAsync(
                service => service.SalonId == request.SalonId && serviceIds.Contains(service.Id),
                cancellationToken) != serviceIds.Length)
            return ServiceOperationStatus.InvalidOwnership;
        if (await writeContext.ServiceTariffItems.AnyAsync(
                item => item.TariffId == tariffId && serviceIds.Contains(item.ServiceId),
                cancellationToken))
            return ServiceOperationStatus.Conflict;

        await writeContext.ServiceTariffItems.AddRangeAsync(
            request.Services.Select(service => ServiceTariffItem.Create(
                request.SalonId,
                tariffId,
                service.ServiceId,
                service.Amount)),
            cancellationToken);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }
}
