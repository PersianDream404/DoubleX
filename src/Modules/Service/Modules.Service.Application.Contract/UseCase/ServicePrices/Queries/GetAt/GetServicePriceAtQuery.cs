using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePrices;

namespace Modules.Service.Application.Contract.UseCase.ServicePrices.Queries.GetAt;

public sealed record GetServicePriceAtQuery(long SalonId, long ServiceId, DateTimeOffset At)
    : IQuery<ServicePriceDto>;
