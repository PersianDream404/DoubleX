namespace Modules.Salon.Persistence.Mapper.Salons;

using Modules.Salon.Application.Contract.DTOs.Salons.Get;
using Modules.Salon.Application.Contract.DTOs.Salons.GetAll;

using System.Linq.Expressions;

public static class SalonMapper
{
    public static Expression<Func<Personnel.Domain.Entities.Salon, GetByIdSalonResponseDto>> ToGetByIdDto()
    {
        return x => new GetByIdSalonResponseDto
        {
            Id = x.Id,
            Name = x.Name,

        };
    }

    public static Expression<Func<Personnel.Domain.Entities.Salon, GetAllSalonResponseDto>> ToGetAllDto()
    {
        return x => new GetAllSalonResponseDto
        {
            Id = x.Id,
            Name = x.Name,

            Status = x.Status,

        };
    }

    public static Expression<Func<Personnel.Domain.Entities.Salon, GetSelectListSalonResponseDto>> ToGetSelectListDto()
    {
        return x => new GetSelectListSalonResponseDto
        {
            Id = x.Id,

            Status = x.Status,
            Name = x.Name,
        };
    }
}
