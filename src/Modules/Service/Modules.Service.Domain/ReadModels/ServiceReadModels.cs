namespace Modules.Service.Domain.ReadModels;

public sealed record ServiceCategoryReadModel(
    long Id,
    long SalonId,
    string Name,
    string? Description,
    bool IsActive);

public sealed record ServiceReadModel(
    long Id,
    long SalonId,
    long CategoryId,
    string Code,
    string Name,
    string? Description,
    int DurationMinutes,
    bool IsActive);

public sealed record ServiceMaterialReadModel(
    long Id,
    long SalonId,
    long ServiceId,
    long MaterialId,
    string MaterialName,
    string BaseUnitName,
    string BaseUnitSymbol,
    decimal QuantityPerService,
    string? Description);

public sealed record ServiceTariffItemReadModel(long ServiceId, string ServiceName, decimal Amount);

public sealed record ServiceTariffReadModel(
    long Id,
    long SalonId,
    string Name,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo,
    IReadOnlyList<ServiceTariffItemReadModel> Services);
