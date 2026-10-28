using Framwork.PagedList;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Modules.Banner.Persistence.Mapper.Banners;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Enums;
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

        var contracts = await _dbContext.PersonnelContracts
            .AsNoTracking()
            .Where(contract => contract.PersonnelId == personnel.Id)
            .OrderByDescending(contract => contract.EffectiveFrom)
            .Select(contract => new PersonnelContractDto(
                contract.Id,
                contract.CooperationType,
                contract.FixedSalaryAmount,
                contract.DefaultCommissionRate,
                contract.EffectiveFrom,
                contract.EffectiveTo,
                contract.IsActive))
            .ToListAsync(ct);
        var activeContract = contracts.FirstOrDefault(contract => contract.IsActive);

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
                ToLegacySalaryType(activeContract?.CooperationType),
                personnel.FinancialInfo.CardNumber.Value,
                personnel.FinancialInfo.AccountNumber,
                activeContract?.FixedSalaryAmount,
                activeContract?.DefaultCommissionRate,
                activeContract?.EffectiveFrom,
                activeContract?.EffectiveTo),
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
            [],
            contracts);
    }

    private static string ToLegacySalaryType(CooperationType? cooperationType) =>
        cooperationType switch
        {
            CooperationType.FixedSalary => SalaryType.Fixed.ToString(),
            CooperationType.Commission => SalaryType.Commission.ToString(),
            CooperationType.FixedAndCommission => SalaryType.FixedAndCommission.ToString(),
            CooperationType.ChairRental => SalaryType.ChairRental.ToString(),
            CooperationType.RoomRental => SalaryType.RoomRental.ToString(),
            CooperationType.Other => string.Empty,
            _ => string.Empty
        };

    public async Task<PagedList<GetAllPersonalResponseDto>> GetAllProjectedAsync(GetAllPersonnelRequestDto request, CancellationToken ct)
    {
        var query = _dbContext.Personnels
            .AsNoTracking()
            .Where(x => x.SalonId == request.SalonId)
            .WhereIf(!string.IsNullOrWhiteSpace(request.Q), x => x.PersonalInfo.FullName.Contains(request.Q!));

        var result = await query.ToPagedListAsync(
            PersonalMapper.ToGetAllDto(),
            request.PageNumber,
            request.PageSize,
            ct);

        return result;
    }
}
