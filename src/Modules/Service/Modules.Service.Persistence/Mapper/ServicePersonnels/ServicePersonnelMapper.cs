using System.Linq.Expressions;
using Modules.Service.Domain.Entities;
using Modules.Service.Domain.ReadModels;

namespace Modules.Service.Persistence.Mapper.ServicePersonnels;

internal static class ServicePersonnelMapper
{
    public static Expression<Func<ServicePersonnel, ServicePersonnelReadModel>> ToReadModel() =>
        item => new ServicePersonnelReadModel(
            item.Id,
            item.SalonId,
            item.ServiceId,
            item.PersonnelId);
}
