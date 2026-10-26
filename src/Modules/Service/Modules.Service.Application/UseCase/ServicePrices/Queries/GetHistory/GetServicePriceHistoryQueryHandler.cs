using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePrices;
using Modules.Service.Application.Contract.Interfaces.ServicePrices;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Application.Contract.UseCase.ServicePrices.Queries.GetHistory;

namespace Modules.Service.Application.UseCase.ServicePrices.Queries.GetHistory;

public sealed class GetServicePriceHistoryQueryHandler(
    IServicePriceRepository repository,
    IServiceRepository serviceRepository)
    : IQueryHandler<GetServicePriceHistoryQuery, IReadOnlyList<ServicePriceDto>>
{
    public async Task<Result<IReadOnlyList<ServicePriceDto>>> Handle(
        GetServicePriceHistoryQuery query,
        CancellationToken cancellationToken)
    {
        if (!await serviceRepository.ServiceOwnedAsync(query.SalonId, query.ServiceId, cancellationToken))
            return Result.NotFound("خدمت موردنظر در این سالن پیدا نشد.");
        return Result.Success(await repository.GetPriceHistoryAsync(query.SalonId, query.ServiceId, cancellationToken));
    }
}

public sealed class GetServicePriceHistoryQueryValidator : AbstractValidator<GetServicePriceHistoryQuery>
{
    public GetServicePriceHistoryQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.ServiceId).GreaterThan(0);
    }
}
