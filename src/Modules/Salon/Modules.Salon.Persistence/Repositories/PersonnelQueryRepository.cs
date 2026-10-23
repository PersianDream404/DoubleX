using Framwork.PagedList;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Modules.Banner.Persistence.Mapper.Banners;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Persistence.Context;

namespace Modules.Personnel.Persistence.Repositories;

public sealed class PersonnelQueryRepository : QueryRepository<Domain.Entities.Personnel>,  IPersonnelQueryRepository
{
    private readonly PersonnelReadDbContext _dbContext;

    public PersonnelQueryRepository(PersonnelReadDbContext context) : base(context)
    {
        _dbContext = context;
    }

    public async Task<GetByIdPersonnelResponseDto?> GetByIdDtoAsync(long id, CancellationToken ct = default)
    {
        var personnel = await _dbContext.Personnels
            .AsNoTracking()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(ct);

        if (personnel is null)
        {
            return null;
        }

        return new GetByIdPersonnelResponseDto(
            personnel.Id,
            personnel.Status,
            personnel.SalonId,
            new PersonalInfoDto(
                personnel.PersonalInfo.FullName,
                personnel.PersonalInfo.NationalCode.Value,
                personnel.PersonalInfo.BirthDate,
                personnel.PersonalInfo.MaritalStatus.ToString(),
                personnel.PersonalInfo.ChildrenCount ?? 0,
                personnel.PersonalInfo.MobilePhone.Value,
                personnel.PersonalInfo.LandlinePhone?.Value,
                personnel.PersonalInfo.Address.Text,
                personnel.PersonalInfo.Address.IsCloseToSalon,
                personnel.PersonalInfo.Picture?.Value),
            new JobInfoDto(
                personnel.JobInfo.JobTitle,
                personnel.JobInfo.PersonnelCode,
                personnel.JobInfo.CooperationStartDate,
                personnel.JobInfo.WorkingShiftType.ToString(),
                personnel.JobInfo.JobPictures.ToArray(),
                personnel.JobInfo.WorkHistory),
            new FinancialInfoDto(
                personnel.FinancialInfo.SalaryType.ToString(),
                personnel.FinancialInfo.CardNumber.Value,
                personnel.FinancialInfo.AccountNumber),
            new HealthInfoDto(
                personnel.HealthInfo.HealthStatus,
                personnel.HealthInfo.Diseases,
                personnel.HealthInfo.AllergyStatus,
                personnel.HealthInfo.Allergies,
                personnel.HealthInfo.PictureCourse.Select(picture => picture.Value).ToArray(),
                personnel.HealthInfo.PictureTest.Select(picture => picture.Value).ToArray()),
            new GuarantorInfoDto(
                personnel.GuarantorInfo.GuarantorName,
                personnel.GuarantorInfo.Relationship,
                personnel.GuarantorInfo.GuarantorMobilePhone.Value,
                personnel.GuarantorInfo.GuarantorLandlinePhone?.Value,
                personnel.GuarantorInfo.PictureGuaranties.Select(picture => picture.Value).ToArray()),
            []);
    }

    public async Task<PagedList<GetAllPersonalResponseDto>> GetAllProjectedAsync(GetAllPersonnelRequestDto request, CancellationToken ct)
    {
        var query = _dbContext.Personnels
            .AsNoTracking()
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), x => x.PersonalInfo.FullName.Contains(request.Q!));

        var result = await query.ToPagedListAsync(
            PersonalMapper.ToGetAllDto(),
            request.PageNumber,
            request.PageSize,
            ct);

        return result;
    }
}
