using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceMaterials;

namespace Modules.Service.Application.Contract.UseCase.ServiceMaterials.Queries.GetAll;

public sealed record GetServiceMaterialsQuery(long SalonId, long ServiceId)
    : IQuery<IReadOnlyList<ServiceMaterialDto>>;
