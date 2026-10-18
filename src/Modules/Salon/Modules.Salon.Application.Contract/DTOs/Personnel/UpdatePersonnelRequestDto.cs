namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record UpdatePersonnelRequestDto(
    PersonalInfoRequestDto PersonalInfo,
    JobInfoRequestDto JobInfo,
    FinancialInfoRequestDto FinancialInfo,
    HealthInfoRequestDto HealthInfo,
    GuarantorInfoRequestDto GuarantorInfo,
    IReadOnlyList<DocumentRequestDto> Documents);
