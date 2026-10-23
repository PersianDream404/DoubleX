namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed class HealthInfoRequestDto
{
    public string? HealthStatus { get; init; }
    public string? AllergyStatus { get; init; }
    public string? Diseases { get; init; }
    public string? Allergies { get; init; }

    public HealthInfoRequestDto(
        string? healthStatus,
        string? allergyStatus,
        string? diseases = null,
        string? allergies = null)
    {
        HealthStatus = healthStatus;
        AllergyStatus = allergyStatus;
        Diseases = diseases;
        Allergies = allergies;
    }

    public HealthInfoRequestDto()
    {
        
    }
}
