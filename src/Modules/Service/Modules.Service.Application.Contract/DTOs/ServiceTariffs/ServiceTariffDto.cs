namespace Modules.Service.Application.Contract.DTOs.ServiceTariffs;

public sealed record ServiceTariffItemDto(long ServiceId, string ServiceName, decimal Amount);

public sealed record ServiceTariffDto(
    long Id,
    long SalonId,
    string Name,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo,
    IReadOnlyList<ServiceTariffItemDto> Services);

public sealed record ServiceTariffRequestDto(
    long SalonId,
    string Name,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo);

public sealed record ServiceTariffServiceRequestDto(long ServiceId, decimal Amount);

public sealed record AddServiceTariffItemsRequestDto(
    long SalonId,
    IReadOnlyList<ServiceTariffServiceRequestDto> Services);
