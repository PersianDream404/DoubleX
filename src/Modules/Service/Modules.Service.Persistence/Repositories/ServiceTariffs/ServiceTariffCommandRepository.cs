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
        var existingTariff = await writeContext.ServiceTariffs
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(
                tariff => tariff.SalonId == request.SalonId && tariff.Name == request.Name.Trim(),
                cancellationToken);
        if (existingTariff is not null)
        {
            if (!existingTariff.Deleted)
                return ServiceOperationStatus.Conflict;

            existingTariff.Deleted = false;
            existingTariff.Update(request.Name, request.ValidFrom, request.ValidTo);
            await writeContext.SaveChangesAsync(cancellationToken);
            return ServiceOperationStatus.Succeeded;
        }

        await writeContext.ServiceTariffs.AddAsync(
            ServiceTariff.Create(request.SalonId, request.Name, request.ValidFrom, request.ValidTo),
            cancellationToken);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> UpdateTariffAsync(
        long id,
        ServiceTariffRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!await writeContext.Set<ServiceSalonReference>()
                .AnyAsync(salon => salon.Id == request.SalonId, cancellationToken))
            return ServiceOperationStatus.NotFound;
        if (request.ValidTo.HasValue && request.ValidTo.Value <= request.ValidFrom)
            return ServiceOperationStatus.InvalidTariffValidity;

        var tariff = await writeContext.ServiceTariffs.SingleOrDefaultAsync(
            item => item.Id == id && item.SalonId == request.SalonId,
            cancellationToken);
        if (tariff is null)
            return ServiceOperationStatus.NotFound;
        if (await writeContext.ServiceTariffs.AnyAsync(
                item => item.SalonId == request.SalonId &&
                        item.Id != id &&
                        item.Name == request.Name.Trim(),
                cancellationToken))
            return ServiceOperationStatus.Conflict;

        tariff.Update(request.Name, request.ValidFrom, request.ValidTo);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> DeleteTariffAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken)
    {
        var tariff = await writeContext.ServiceTariffs.SingleOrDefaultAsync(
            item => item.Id == id && item.SalonId == salonId,
            cancellationToken);
        if (tariff is null)
            return ServiceOperationStatus.NotFound;

        var items = await writeContext.ServiceTariffItems
            .Where(item => item.TariffId == id && item.SalonId == salonId)
            .ToListAsync(cancellationToken);
        foreach (var item in items)
            item.Deleted = true;
        tariff.Deleted = true;

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

        var previousItems = await writeContext.ServiceTariffItems
            .IgnoreQueryFilters()
            .Where(item => item.TariffId == tariffId &&
                           item.SalonId == request.SalonId &&
                           serviceIds.Contains(item.ServiceId))
            .ToListAsync(cancellationToken);
        var newItems = new List<ServiceTariffItem>();
        foreach (var service in request.Services)
        {
            var previousItem = previousItems.SingleOrDefault(item => item.ServiceId == service.ServiceId);
            if (previousItem is null)
            {
                newItems.Add(ServiceTariffItem.Create(
                    request.SalonId,
                    tariffId,
                    service.ServiceId,
                    service.Amount));
                continue;
            }

            previousItem.Deleted = false;
            previousItem.UpdateAmount(service.Amount);
        }
        await writeContext.ServiceTariffItems.AddRangeAsync(newItems, cancellationToken);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> UpdateServiceAmountAsync(
        long tariffId,
        long serviceId,
        UpdateServiceTariffItemRequestDto request,
        CancellationToken cancellationToken)
    {
        var item = await writeContext.ServiceTariffItems.SingleOrDefaultAsync(
            serviceTariffItem => serviceTariffItem.TariffId == tariffId &&
                                 serviceTariffItem.ServiceId == serviceId &&
                                 serviceTariffItem.SalonId == request.SalonId,
            cancellationToken);
        if (item is null)
            return ServiceOperationStatus.NotFound;

        item.UpdateAmount(request.Amount);
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

    public async Task<ServiceOperationStatus> RemoveServiceAsync(
        long tariffId,
        long serviceId,
        long salonId,
        CancellationToken cancellationToken)
    {
        var item = await writeContext.ServiceTariffItems.SingleOrDefaultAsync(
            serviceTariffItem => serviceTariffItem.TariffId == tariffId &&
                                 serviceTariffItem.ServiceId == serviceId &&
                                 serviceTariffItem.SalonId == salonId,
            cancellationToken);
        if (item is null)
            return ServiceOperationStatus.NotFound;

        item.Deleted = true;
        await writeContext.SaveChangesAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }
}
