using System.Linq.Expressions;
using Modules.Service.Domain.Entities;
using Modules.Service.Domain.ReadModels;

namespace Modules.Service.Persistence.Mapper.ServiceTariffs;

public static class ServiceTariffMapper
{
    public static Expression<Func<ServiceTariff, ServiceTariffReadModel>> ToReadModel() =>
        tariff => new ServiceTariffReadModel(
            tariff.Id,
            tariff.SalonId,
            tariff.Name,
            tariff.ValidFrom,
            tariff.ValidTo,
            tariff.Items
                .OrderBy(item => item.Service.Name)
                .Select(item => new ServiceTariffItemReadModel(
                    item.ServiceId,
                    item.Service.Name,
                    item.Amount))
                .ToList());
}
