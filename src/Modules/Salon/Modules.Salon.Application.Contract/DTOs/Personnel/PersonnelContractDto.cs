using Modules.Personnel.Domain.Enums;

namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record PersonnelContractDto(
    long Id,
    CooperationType CooperationType,
    decimal? FixedSalaryAmount,
    decimal? DefaultCommissionRate,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo,
    bool IsActive);
