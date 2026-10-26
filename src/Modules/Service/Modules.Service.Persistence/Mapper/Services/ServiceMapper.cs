using System.Linq.Expressions;
using Modules.Service.Domain.ReadModels;
using ServiceEntity = Modules.Service.Domain.Entities.Service;

namespace Modules.Service.Persistence.Mapper.Services;

public static class ServiceMapper
{
    public static Expression<Func<ServiceEntity, ServiceReadModel>> ToReadModel() =>
        service => new ServiceReadModel(
            service.Id,
            service.SalonId,
            service.CategoryId,
            service.Code,
            service.Name,
            service.Description,
            service.DurationMinutes,
            service.Status);
}
