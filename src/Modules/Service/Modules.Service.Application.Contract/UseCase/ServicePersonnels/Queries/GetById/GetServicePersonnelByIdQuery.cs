using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;

namespace Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetById;

public sealed record GetServicePersonnelByIdQuery(long SalonId, long ServiceId, long Id)
    : IQuery<ServicePersonnelDto>;
