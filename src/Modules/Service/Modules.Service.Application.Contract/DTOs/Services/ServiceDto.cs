namespace Modules.Service.Application.Contract.DTOs.Services;

public sealed record ServiceDto(
    long Id,
    long SalonId,
    long CategoryId,
    string Code,
    string Name,
    string? Description,
    int DurationMinutes,
    bool IsActive);

public sealed record ServiceRequestDto(
    long SalonId,
    long CategoryId,
    string Code,
    string Name,
    string? Description,
    int DurationMinutes);

public sealed record UpdateServiceRequestDto(
    long SalonId,
    long CategoryId,
    string Code,
    string Name,
    string? Description,
    int DurationMinutes,
    bool IsActive);
