using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetAll;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.Services.Queries.GetAll;

public sealed class GetServicesQueryHandler(
    IServiceQueryRepository repository,
    IServiceCategoryQueryRepository categoryRepository)
    : IQueryHandler<GetServicesQuery, IReadOnlyList<ServiceDto>>
{
    public async Task<Result<IReadOnlyList<ServiceDto>>> Handle(
        GetServicesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await categoryRepository.SalonExistsAsync(query.SalonId, cancellationToken))
            return Result.NotFound("سالن موردنظر پیدا نشد.");
        var services = await repository.GetServicesAsync(
            query.SalonId,
            query.SearchTerm,
            cancellationToken);
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

public sealed class GetServicesQueryValidator : AbstractValidator<GetServicesQuery>
{
    public GetServicesQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.SearchTerm).MaximumLength(250);
    }
}
