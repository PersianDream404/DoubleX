using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetById;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.ServiceTariffs.Queries.GetById;

public sealed class GetServiceTariffByIdQueryHandler(IServiceTariffQueryRepository repository)
    : IQueryHandler<GetServiceTariffByIdQuery, ServiceTariffDto>
{
    public async Task<Result<ServiceTariffDto>> Handle(
        GetServiceTariffByIdQuery query,
        CancellationToken cancellationToken)
    {
        var tariff = await repository.GetTariffByIdAsync(query.Id, cancellationToken);
        return tariff is null
            ? Result.NotFound("تعرفه موردنظر پیدا نشد.")
            : Result.Success(new ServiceTariffDto(
                tariff.Id,
                tariff.SalonId,
                tariff.Name,
                tariff.ValidFrom,
                tariff.ValidTo,
                tariff.Services
                    .Select(service => new ServiceTariffItemDto(
                        service.ServiceId,
                        service.ServiceName,
                        service.Amount))
                    .ToList()));
    }
}

public sealed class GetServiceTariffByIdQueryValidator : AbstractValidator<GetServiceTariffByIdQuery>
{
    public GetServiceTariffByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}
