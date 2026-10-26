namespace Modules.Service.Application.Contract.DTOs.ServiceCategories;

public sealed record ServiceCategoryDto(
    long Id,
    long SalonId,
    string Name,
    string? Description,
    bool IsActive);

public sealed record ServiceCategoryRequestDto(
    long SalonId,
    string Name,
    string? Description);

public sealed record UpdateServiceCategoryRequestDto(
    long SalonId,
    string Name,
    string? Description,
    bool IsActive);
