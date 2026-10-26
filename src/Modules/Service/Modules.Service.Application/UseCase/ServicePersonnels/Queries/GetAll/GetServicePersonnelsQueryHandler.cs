using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetAll;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.ServicePersonnels.Queries.GetAll;

public sealed class GetServicePersonnelsQueryHandler(
    IServicePersonnelQueryRepository repository,
    IServiceQueryRepository serviceRepository)
    : IQueryHandler<GetServicePersonnelsQuery, IReadOnlyList<ServicePersonnelDto>>
{
    public async Task<Result<IReadOnlyList<ServicePersonnelDto>>> Handle(
        GetServicePersonnelsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await serviceRepository.ServiceOwnedAsync(
                query.SalonId,
                query.ServiceId,
                cancellationToken))
        {
            return Result.NotFound("خدمت موردنظر در این سالن پیدا نشد.");
        }

        var personnel = await repository.GetByServiceAsync(
            query.SalonId,
            query.ServiceId,
            cancellationToken);
        return Result.Success<IReadOnlyList<ServicePersonnelDto>>(personnel
            .Select(item => new ServicePersonnelDto(
                item.Id,
                item.SalonId,
                item.ServiceId,
                item.PersonnelId))
            .ToList());
    }
}

public sealed class GetServicePersonnelsQueryValidator
    : AbstractValidator<GetServicePersonnelsQuery>
{
    public GetServicePersonnelsQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.ServiceId).GreaterThan(0);
    }
}
