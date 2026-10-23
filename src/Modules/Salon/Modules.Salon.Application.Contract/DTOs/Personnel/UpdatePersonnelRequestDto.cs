namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record UpdatePersonnelRequestDto(
    long SalonId,
    bool Status,
    PersonalInfoRequestDto PersonalInfo,
    JobInfoRequestDto JobInfo,
    FinancialInfoRequestDto FinancialInfo,
    HealthInfoRequestDto HealthInfo,
    GuarantorInfoRequestDto GuarantorInfo,
    IReadOnlyList<DocumentRequestDto> Documents);
