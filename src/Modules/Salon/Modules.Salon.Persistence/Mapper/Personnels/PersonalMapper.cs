namespace Modules.Banner.Persistence.Mapper.Banners;

using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Domain.Entities;
using System.Linq.Expressions;

public static class PersonalMapper
{
    //public static Expression<Func<BannerEntity, GetByIdBannerResponseDto>> ToGetByIdDto()
    //{
    //    return x => new GetByIdBannerResponseDto
    //    {
    //        Id = x.Id,
    //        Description = x.Description,
    //        EndDate = x.EndDate,
    //        ImageUrl = x.ImageUrl,
    //        Order = x.Order,
    //        StartDate = x.StartDate,
    //        Url = x.Url,
    //        Title = x.Title,
    //        Status = x.Status
    //    };
    //}

    public static Expression<Func<Personnel, GetAllPersonalResponseDto>> ToGetAllDto()
    {
        return x => new GetAllPersonalResponseDto(

            Id: x.Id,
            Status: x.Status,
            FullName: x.PersonalInfo.FullName,
            NationalCode: x.PersonalInfo.NationalCode.Value,
            JobTitle: x.JobInfo.JobTitle,
            CardNumber:x.FinancialInfo.CardNumber.Value,
            BirthDate: x.PersonalInfo.BirthDate,
            MobilePhone: x.PersonalInfo.MobilePhone.Value
            
            );
    }

    //public static Expression<Func<BannerEntity, GetSelectListBannerResponseDto>> ToGetSelectListDto()
    //{
    //    return x => new GetSelectListBannerResponseDto
    //    {
    //        Id = x.Id,

    //        ImageUrl = x.ImageUrl,
    //        Status = x.Status,
    //        Title = x.Title,
    //    };
    //}
}
