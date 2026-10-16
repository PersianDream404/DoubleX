using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Banner.Application.Contract.DTOs.Banners.Get;
using Modules.Banner.Application.Contract.DTOs.Banners.GetAll;

namespace Modules.Banner.Application.Contract.UseCase.Banners.Queries;

public record GetAllBannerQuery(GetAllBannerRequestDto request) : 
    IQuery<PagedList<GetAllBannerResponseDto>>;
public record GetSelectListBannerQuery(GetSelectListBannerRequestDto request) :
    IQuery<PagedList<GetSelectListBannerResponseDto>>;
public record GetByIdBannerQuery(long Id) : IQuery<GetByIdBannerResponseDto>;
