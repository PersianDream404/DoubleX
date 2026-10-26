using System.Linq.Expressions;
using Modules.Service.Application.Contract.DTOs.Services;
using ServiceEntity = Modules.Service.Domain.Entities.Service;

namespace Modules.Service.Persistence.Mapper.Services;

public static class ServiceMapper
{
    public static Expression<Func<ServiceEntity, ServiceDto>> ToDto() =>
        service => new ServiceDto(
            service.Id,
            service.SalonId,
            service.CategoryId,
            service.Code,
            service.Name,
            service.Description,
            service.DurationMinutes,
            service.Status);
}
