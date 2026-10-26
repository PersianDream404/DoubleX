using Microsoft.EntityFrameworkCore;
using Modules.Service.Domain.Interfaces;
using Modules.Service.Domain.ReadModels;
using Modules.Service.Persistence.Context;
using Modules.Service.Persistence.Mapper.ServiceTariffs;

namespace Modules.Service.Persistence.Repositories.ServiceTariffs;

public sealed class ServiceTariffQueryRepository(ServiceReadDbContext readContext)
    : IServiceTariffQueryRepository
{
    public Task<bool> SalonExistsAsync(long salonId, CancellationToken cancellationToken) =>
        readContext.Set<ServiceSalonReference>().AnyAsync(salon => salon.Id == salonId, cancellationToken);

    public async Task<IReadOnlyList<ServiceTariffReadModel>> GetTariffsAsync(
        long salonId,
        CancellationToken cancellationToken)
        => await readContext.ServiceTariffs
            .AsNoTracking()
            .Where(tariff => tariff.SalonId == salonId)
            .OrderBy(tariff => tariff.Name)
            .Select(ServiceTariffMapper.ToReadModel())
            .ToListAsync(cancellationToken);

    public async Task<ServiceTariffReadModel?> GetTariffByIdAsync(
        long id,
        CancellationToken cancellationToken)
    {
        return await readContext.ServiceTariffs
            .AsNoTracking()
            .Where(item => item.Id == id)
            .Select(ServiceTariffMapper.ToReadModel())
            .SingleOrDefaultAsync(cancellationToken);
    }
}
