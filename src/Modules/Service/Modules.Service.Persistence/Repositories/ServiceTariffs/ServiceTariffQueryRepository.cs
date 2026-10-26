using Microsoft.EntityFrameworkCore;
using Modules.Service.Domain.Interfaces;
using Modules.Service.Domain.ReadModels;
using Modules.Service.Persistence.Context;

namespace Modules.Service.Persistence.Repositories.ServiceTariffs;

public sealed class ServiceTariffQueryRepository(ServiceReadDbContext readContext)
    : IServiceTariffQueryRepository
{
    public Task<bool> SalonExistsAsync(long salonId, CancellationToken cancellationToken) =>
        readContext.Set<ServiceSalonReference>().AnyAsync(salon => salon.Id == salonId, cancellationToken);

    public async Task<IReadOnlyList<ServiceTariffReadModel>> GetTariffsAsync(
        long salonId,
        CancellationToken cancellationToken)
    {
        var tariffs = await readContext.ServiceTariffs
            .AsNoTracking()
            .Where(tariff => tariff.SalonId == salonId)
            .Include(tariff => tariff.Items)
                .ThenInclude(item => item.Service)
            .OrderBy(tariff => tariff.Name)
            .ToListAsync(cancellationToken);

        return tariffs.Select(tariff => new ServiceTariffReadModel(
                tariff.Id,
                tariff.SalonId,
                tariff.Name,
                tariff.ValidFrom,
                tariff.ValidTo,
                tariff.Items
                    .OrderBy(item => item.Service.Name)
                    .Select(item => new ServiceTariffItemReadModel(item.ServiceId, item.Service.Name, item.Amount))
                    .ToList()))
            .ToList();
    }
}
