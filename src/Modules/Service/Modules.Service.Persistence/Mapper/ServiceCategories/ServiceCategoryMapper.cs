using System.Linq.Expressions;
using Modules.Service.Domain.Entities;
using Modules.Service.Domain.ReadModels;

namespace Modules.Service.Persistence.Mapper.ServiceCategories;

public static class ServiceCategoryMapper
{
    public static Expression<Func<ServiceCategory, ServiceCategoryReadModel>> ToReadModel() =>
        category => new ServiceCategoryReadModel(
            category.Id,
            category.SalonId,
            category.Name,
            category.Description,
            category.Status);
}
