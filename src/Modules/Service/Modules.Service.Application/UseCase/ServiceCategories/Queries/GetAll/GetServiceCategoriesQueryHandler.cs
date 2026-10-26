using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Queries.GetAll;

namespace Modules.Service.Application.UseCase.ServiceCategories.Queries.GetAll;

public sealed class GetServiceCategoriesQueryHandler(IServiceCategoryRepository repository)
    : IQueryHandler<GetServiceCategoriesQuery, IReadOnlyList<ServiceCategoryDto>>
{
    public async Task<Result<IReadOnlyList<ServiceCategoryDto>>> Handle(
        GetServiceCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await repository.SalonExistsAsync(query.SalonId, cancellationToken))
            return Result.NotFound("سالن موردنظر پیدا نشد.");
        return Result.Success(await repository.GetCategoriesAsync(query.SalonId, cancellationToken));
    }
}

public sealed class GetServiceCategoriesQueryValidator : AbstractValidator<GetServiceCategoriesQuery>
{
    public GetServiceCategoriesQueryValidator() => RuleFor(query => query.SalonId).GreaterThan(0);
}
