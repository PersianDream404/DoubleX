using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Reception.Application.Contract.DTOs.Receptions.GetAll;

namespace Modules.Reception.Application.Contract.UseCase.Receptions.Queries.GetAll;

public sealed record GetReceptionsQuery(GetReceptionsRequestDto Request)
    : IQuery<PagedList<ReceptionListItemDto>>;
