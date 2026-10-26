using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetActive;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.Services.Queries.GetActive;

public sealed class GetActiveServicesQueryHandler(
    IServiceQueryRepository repository,
    IServiceCategoryQueryRepository categoryRepository)
    : IQueryHandler<GetActiveServicesQuery, IReadOnlyList<ServiceDto>>
{
    public async Task<Result<IReadOnlyList<ServiceDto>>> Handle(
        GetActiveServicesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await categoryRepository.SalonExistsAsync(query.SalonId, cancellationToken))
            return Result.NotFound("سالن موردنظر پیدا نشد.");
        var services = await repository.GetActiveServicesAsync(query.SalonId, cancellationToken);
        return Result.Success<IReadOnlyList<ServiceDto>>(services
            .Select(service => new ServiceDto(
                service.Id,
                service.SalonId,
                service.CategoryId,
                service.Code,
                service.Name,
                service.Description,
                service.DurationMinutes,
                service.IsActive))
            .ToList());
    }
}

public sealed class GetActiveServicesQueryValidator : AbstractValidator<GetActiveServicesQuery>
{
    public GetActiveServicesQueryValidator() => RuleFor(query => query.SalonId).GreaterThan(0);
}
