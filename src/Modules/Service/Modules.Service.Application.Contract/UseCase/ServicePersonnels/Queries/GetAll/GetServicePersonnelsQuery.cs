using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;

namespace Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetAll;

public sealed record GetServicePersonnelsQuery(long SalonId, long ServiceId)
    : IQuery<IReadOnlyList<ServicePersonnelDto>>;
