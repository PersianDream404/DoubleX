using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePrices;

namespace Modules.Service.Application.Contract.UseCase.ServicePrices.Queries.GetHistory;

public sealed record GetServicePriceHistoryQuery(long SalonId, long ServiceId)
    : IQuery<IReadOnlyList<ServicePriceDto>>;
