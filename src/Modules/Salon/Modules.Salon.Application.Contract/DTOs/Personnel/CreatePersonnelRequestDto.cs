namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record CreatePersonnelRequestDto(
    long SalonId,
    PersonalInfoRequestDto PersonalInfo,
    JobInfoRequestDto JobInfo,
    FinancialInfoRequestDto FinancialInfo,
    HealthInfoRequestDto HealthInfo,
    GuarantorInfoRequestDto GuarantorInfo,
    IReadOnlyList<DocumentRequestDto> Documents,
    bool Status = true);
