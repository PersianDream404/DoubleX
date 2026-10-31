namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record PersonnelContractExpiryDto(
    long PersonnelId,
    string FullName,
    DateOnly EffectiveTo);

public sealed record PersonnelContractExpirySummaryDto(
    int TotalCount,
    IReadOnlyList<PersonnelContractExpiryDto> Items);
