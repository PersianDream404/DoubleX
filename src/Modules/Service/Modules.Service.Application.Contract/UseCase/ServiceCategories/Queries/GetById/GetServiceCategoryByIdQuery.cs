using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;

namespace Modules.Service.Application.Contract.UseCase.ServiceCategories.Queries.GetById;

public sealed record GetServiceCategoryByIdQuery(long Id) : IQuery<ServiceCategoryDto>;
