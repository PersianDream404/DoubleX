namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record HealthInfoDto(
    string? HealthStatus,
    string? AllergyStatus);
