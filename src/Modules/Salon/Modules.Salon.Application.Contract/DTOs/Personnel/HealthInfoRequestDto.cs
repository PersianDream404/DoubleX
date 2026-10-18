namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed class HealthInfoRequestDto
{
    public string? HealthStatus { get; init; }
    public string? AllergyStatus { get; init; }

    public HealthInfoRequestDto(
        string? healthStatus,
        string? allergyStatus)
    {
        HealthStatus = healthStatus;
        AllergyStatus = allergyStatus;
    }

    public HealthInfoRequestDto()
    {
        
    }
}
