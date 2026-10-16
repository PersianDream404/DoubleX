using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Personnel.Application.Contract.DTOs.Personnel;

namespace Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;

public record GetPersonnelByIdQuery(long Id) : IQuery<GetPersonnelDto>;
public record GetAllPersonnelQuery(GetAllPersonnelRequestDto request) : IQuery<PagedList<GetAllPersonalResponseDto>>;
