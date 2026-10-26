using System.Linq.Expressions;
using Modules.Service.Application.Contract.DTOs.ServicePrices;
using Modules.Service.Domain.Entities;

namespace Modules.Service.Persistence.Mapper.ServicePrices;

public static class ServicePriceMapper
{
    public static Expression<Func<ServicePrice, ServicePriceDto>> ToDto() =>
        price => new ServicePriceDto(
            price.Id,
            price.SalonId,
            price.ServiceId,
            price.Amount,
            price.ValidFrom,
            price.ValidTo);
}
