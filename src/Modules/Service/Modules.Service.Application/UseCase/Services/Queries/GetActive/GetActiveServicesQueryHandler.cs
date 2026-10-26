using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetActive;

namespace Modules.Service.Application.UseCase.Services.Queries.GetActive;

public sealed class GetActiveServicesQueryHandler(
    IServiceRepository repository,
    IServiceCategoryRepository categoryRepository)
    : IQueryHandler<GetActiveServicesQuery, IReadOnlyList<ServiceDto>>
{
    public async Task<Result<IReadOnlyList<ServiceDto>>> Handle(
        GetActiveServicesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await categoryRepository.SalonExistsAsync(query.SalonId, cancellationToken))
            return Result.NotFound("سالن موردنظر پیدا نشد.");
        return Result.Success(await repository.GetActiveServicesAsync(query.SalonId, cancellationToken));
    }
}

public sealed class GetActiveServicesQueryValidator : AbstractValidator<GetActiveServicesQuery>
{
    public GetActiveServicesQueryValidator() => RuleFor(query => query.SalonId).GreaterThan(0);
}
