using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetAll;

namespace Modules.Service.Application.UseCase.Services.Queries.GetAll;

public sealed class GetServicesQueryHandler(
    IServiceRepository repository,
    IServiceCategoryRepository categoryRepository)
    : IQueryHandler<GetServicesQuery, IReadOnlyList<ServiceDto>>
{
    public async Task<Result<IReadOnlyList<ServiceDto>>> Handle(
        GetServicesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await categoryRepository.SalonExistsAsync(query.SalonId, cancellationToken))
            return Result.NotFound("سالن موردنظر پیدا نشد.");
        return Result.Success(await repository.GetServicesAsync(query.SalonId, cancellationToken));
    }
}

public sealed class GetServicesQueryValidator : AbstractValidator<GetServicesQuery>
{
    public GetServicesQueryValidator() => RuleFor(query => query.SalonId).GreaterThan(0);
}
