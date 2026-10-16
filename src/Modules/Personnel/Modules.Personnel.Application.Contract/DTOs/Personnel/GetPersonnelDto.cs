using Framwork.PagedList;

namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record GetPersonnelDto(
    long Id,
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