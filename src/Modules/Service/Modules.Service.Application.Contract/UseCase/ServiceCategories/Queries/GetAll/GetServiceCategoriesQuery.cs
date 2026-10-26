using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;

namespace Modules.Service.Application.Contract.UseCase.ServiceCategories.Queries.GetAll;

public sealed record GetServiceCategoriesQuery(long SalonId) : IQuery<IReadOnlyList<ServiceCategoryDto>>;
