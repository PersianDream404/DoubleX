using Framwork.PagedList;

namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record GetByIdPersonnelResponseDto(
    long Id,
    bool Status,
    PersonalInfoDto PersonalInfo,
    JobInfoDto JobInfo,
    FinancialInfoDto FinancialInfo,
    HealthInfoDto HealthInfo,
    GuarantorInfoDto GuarantorInfo,
    IReadOnlyList<DocumentDto> Documents);






public class GetAllPersonnelRequestDto : PagedParamData
{
    public string? Q { get; set; }
}