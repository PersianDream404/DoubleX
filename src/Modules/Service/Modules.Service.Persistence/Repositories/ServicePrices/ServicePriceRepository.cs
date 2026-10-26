using System.Data;
using Microsoft.EntityFrameworkCore;
using Modules.Service.Application.Contract.DTOs.ServicePrices;
using Modules.Service.Application.Contract.Enums;
using Modules.Service.Application.Contract.Interfaces.ServicePrices;
using Modules.Service.Domain.Entities;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Mapper.ServicePrices;

namespace Modules.Service.Persistence.Repositories.ServicePrices;

public sealed class ServicePriceRepository(
    ServiceWriteDbContext writeContext,
    ServiceReadDbContext readContext) : IServicePriceRepository
{
    public async Task<IReadOnlyList<ServicePriceDto>> GetPriceHistoryAsync(
        long salonId,
        long serviceId,
        CancellationToken cancellationToken) =>
        await readContext.ServicePrices
            .AsNoTracking()
            .Where(price => price.SalonId == salonId && price.ServiceId == serviceId)
            .OrderByDescending(price => price.ValidFrom)
            .Select(ServicePriceMapper.ToDto())
            .ToListAsync(cancellationToken);

    public Task<ServicePriceDto?> GetPriceAtAsync(
        long salonId,
        long serviceId,
        DateTimeOffset at,
        CancellationToken cancellationToken) =>
        readContext.ServicePrices
            .AsNoTracking()
            .Where(price => price.SalonId == salonId &&
                            price.ServiceId == serviceId &&
                            price.ValidFrom <= at &&
                            (price.ValidTo == null || at < price.ValidTo))
            .Select(ServicePriceMapper.ToDto())
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<ServiceOperationStatus> AddPriceAsync(
        CreateServicePriceRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.ValidTo.HasValue && request.ValidTo.Value <= request.ValidFrom)
            return ServiceOperationStatus.InvalidPriceRange;

        await using var transaction = await writeContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        if (!await writeContext.Services.AnyAsync(
                service => service.Id == request.ServiceId &&
                           service.SalonId == request.SalonId,
                cancellationToken))
            return ServiceOperationStatus.NotFound;

        var overlappingPrices = await writeContext.ServicePrices
            .Where(price => price.SalonId == request.SalonId &&
                            price.ServiceId == request.ServiceId &&
                            (!request.ValidTo.HasValue || price.ValidFrom < request.ValidTo.Value) &&
                            (!price.ValidTo.HasValue || request.ValidFrom < price.ValidTo.Value))
            .ToListAsync(cancellationToken);
        var openPriorPrice = overlappingPrices.SingleOrDefault(price =>
            !price.ValidTo.HasValue &&
            price.ValidFrom < request.ValidFrom);
        if (openPriorPrice is not null)
        {
            openPriorPrice.CloseAt(request.ValidFrom);
            overlappingPrices.Remove(openPriorPrice);
        }
        if (overlappingPrices.Count != 0)
            return ServiceOperationStatus.Conflict;

        await writeContext.ServicePrices.AddAsync(
            ServicePrice.Create(
                request.SalonId,
                request.ServiceId,
                request.Amount,
                request.ValidFrom,
                request.ValidTo),
            cancellationToken);
        await writeContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return ServiceOperationStatus.Succeeded;
    }

}
