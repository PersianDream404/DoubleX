using Framwork.Bus.Query;
using Modules.Reception.Application.Contract.DTOs.Receptions.Get;

namespace Modules.Reception.Application.Contract.UseCase.Receptions.Queries.GetById;

public sealed record GetReceptionByIdQuery(long Id, long SalonId) : IQuery<ReceptionDetailsDto>;
