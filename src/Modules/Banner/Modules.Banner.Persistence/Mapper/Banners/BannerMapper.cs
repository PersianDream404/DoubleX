namespace Modules.Banner.Persistence.Mapper.Banners;

using Modules.Banner.Application.Contract.DTOs.Banners.Get;
using Modules.Banner.Application.Contract.DTOs.Banners.GetAll;
using Modules.Banner.Domain.Entities;
using System.Linq.Expressions;

public static class BannerMapper
{
    public static Expression<Func<BannerEntity, GetByIdBannerResponseDto>> ToGetByIdDto()
    {
        return x => new GetByIdBannerResponseDto
        {
            Id = x.Id,
            Description = x.Description,
            EndDate = x.EndDate,
            ImageUrl = x.ImageUrl,
            Order = x.Order,
            StartDate = x.StartDate,
            Url = x.Url,
            Title = x.Title,
            Status = x.Status,
            Type=x.Type,
        };
    }

    public static Expression<Func<BannerEntity, GetAllBannerResponseDto>> ToGetAllDto()
    {
        return x => new GetAllBannerResponseDto
        {
            Id = x.Id,
            Description = x.Description,
            EndDate = x.EndDate,
            ImageUrl = x.ImageUrl,
            Order = x.Order,
            StartDate = x.StartDate,
            Url = x.Url,
            Title = x.Title,
            Status = x.Status,
           Type = x.Type,
        };
    }

    public static Expression<Func<BannerEntity, GetSelectListBannerResponseDto>> ToGetSelectListDto()
    {
        return x => new GetSelectListBannerResponseDto
        {
            Id = x.Id,

            ImageUrl = x.ImageUrl,
            Status = x.Status,
            Title = x.Title,
        };
    }
}
