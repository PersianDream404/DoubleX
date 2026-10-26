using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;

namespace Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetByPersonnel;

public sealed record GetServicePersonnelAssignmentsQuery(long SalonId, long PersonnelId)
    : IQuery<IReadOnlyList<ServicePersonnelDto>>;
