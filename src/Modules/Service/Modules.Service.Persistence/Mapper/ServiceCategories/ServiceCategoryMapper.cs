using System.Linq.Expressions;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Domain.Entities;

namespace Modules.Service.Persistence.Mapper.ServiceCategories;

public static class ServiceCategoryMapper
{
    public static Expression<Func<ServiceCategory, ServiceCategoryDto>> ToDto() =>
        category => new ServiceCategoryDto(
            category.Id,
            category.SalonId,
            category.Name,
            category.Description,
            category.Status);
}
