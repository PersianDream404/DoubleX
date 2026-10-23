namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record HealthInfoDto(
    bool HealthStatus,
    string? Diseases,
    bool AllergyStatus,
    string? Allergies,
    IReadOnlyList<string> PictureCourse,
    IReadOnlyList<string> PictureTest);
