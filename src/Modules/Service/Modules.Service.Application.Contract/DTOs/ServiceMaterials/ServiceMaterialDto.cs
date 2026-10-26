namespace Modules.Service.Application.Contract.DTOs.ServiceMaterials;

public sealed record ServiceMaterialDto(
    long Id,
    long SalonId,
    long ServiceId,
    long MaterialId,
    string MaterialName,
    string BaseUnitName,
    string BaseUnitSymbol,
    decimal QuantityPerService,
    string? Description);

public sealed record ServiceMaterialRequestDto(
    long SalonId,
    long ServiceId,
    long MaterialId,
    decimal QuantityPerService,
    string? Description);
