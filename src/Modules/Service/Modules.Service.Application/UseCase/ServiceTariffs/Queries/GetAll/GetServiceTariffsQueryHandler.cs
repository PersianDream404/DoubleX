using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetAll;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.ServiceTariffs.Queries.GetAll;

public sealed class GetServiceTariffsQueryHandler(IServiceTariffQueryRepository repository)
    : IQueryHandler<GetServiceTariffsQuery, IReadOnlyList<ServiceTariffDto>>
{
    public async Task<Result<IReadOnlyList<ServiceTariffDto>>> Handle(
        GetServiceTariffsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await repository.SalonExistsAsync(query.SalonId, cancellationToken))
            return Result.NotFound("سالن موردنظر پیدا نشد.");
        var tariffs = await repository.GetTariffsAsync(query.SalonId, cancellationToken);
        return Result.Success<IReadOnlyList<ServiceTariffDto>>(tariffs
            .Select(tariff => new ServiceTariffDto(
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
                    .ToList()))
            .ToList());
    }
}

public sealed class GetServiceTariffsQueryValidator : AbstractValidator<GetServiceTariffsQuery>
{
    public GetServiceTariffsQueryValidator() =>
        RuleFor(query => query.SalonId).GreaterThan(0);
}
