namespace Modules.Service.Application.Contract.DTOs.ServicePrices;

public sealed record ServicePriceDto(
    long Id,
    long SalonId,
    long ServiceId,
    decimal Amount,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo);

public sealed record CreateServicePriceRequestDto(
    long SalonId,
    long ServiceId,
    decimal Amount,
    DateTimeOffset ValidFrom,
    DateTimeOffset? ValidTo);
