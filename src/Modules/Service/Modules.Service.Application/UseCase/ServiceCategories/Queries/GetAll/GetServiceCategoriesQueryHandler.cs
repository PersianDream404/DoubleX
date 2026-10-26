using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Queries.GetAll;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.ServiceCategories.Queries.GetAll;

public sealed class GetServiceCategoriesQueryHandler(IServiceCategoryQueryRepository repository)
    : IQueryHandler<GetServiceCategoriesQuery, IReadOnlyList<ServiceCategoryDto>>
{
    public async Task<Result<IReadOnlyList<ServiceCategoryDto>>> Handle(
        GetServiceCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        if (!await repository.SalonExistsAsync(query.SalonId, cancellationToken))
            return Result.NotFound("سالن موردنظر پیدا نشد.");
        var categories = await repository.GetCategoriesAsync(query.SalonId, cancellationToken);
        return Result.Success<IReadOnlyList<ServiceCategoryDto>>(categories
            .Select(category => new ServiceCategoryDto(
                category.Id,
                category.SalonId,
                category.Name,
                category.Description,
                category.IsActive))
            .ToList());
    }
}

public sealed class GetServiceCategoriesQueryValidator : AbstractValidator<GetServiceCategoriesQuery>
{
    public GetServiceCategoriesQueryValidator() => RuleFor(query => query.SalonId).GreaterThan(0);
}
