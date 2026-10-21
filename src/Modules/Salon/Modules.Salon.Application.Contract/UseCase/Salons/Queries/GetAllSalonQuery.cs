using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Salon.Application.Contract.DTOs.Salons.Get;
using Modules.Salon.Application.Contract.DTOs.Salons.GetAll;

namespace Modules.Salon.Application.Contract.UseCase.Salons.Queries;

public record GetAllSalonQuery(GetAllSalonRequestDto request) : 
    IQuery<PagedList<GetAllSalonResponseDto>>;
public record GetSelectListSalonQuery(GetSelectListSalonRequestDto request) :
    IQuery<PagedList<GetSelectListSalonResponseDto>>;
public record GetByIdSalonQuery(long Id) : IQuery<GetByIdSalonResponseDto>;
