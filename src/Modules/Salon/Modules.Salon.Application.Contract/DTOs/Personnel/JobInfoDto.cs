namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record JobInfoDto(
    string JobTitle,
    string PersonnelCode,
    DateOnly CooperationStartDate,
    string WorkingShiftType,
    IReadOnlyList<string> Skills,
    string? WorkHistory);
