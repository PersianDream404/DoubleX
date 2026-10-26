using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Queries.GetById;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.ServiceCategories.Queries.GetById;

public sealed class GetServiceCategoryByIdQueryHandler(IServiceCategoryQueryRepository repository)
    : IQueryHandler<GetServiceCategoryByIdQuery, ServiceCategoryDto>
{
    public async Task<Result<ServiceCategoryDto>> Handle(
        GetServiceCategoryByIdQuery query,
        CancellationToken cancellationToken)
    {
        var category = await repository.GetCategoryByIdAsync(query.Id, cancellationToken);
        return category is null
            ? Result.NotFound("دسته‌بندی خدمت موردنظر پیدا نشد.")
            : Result.Success(new ServiceCategoryDto(
                category.Id,
                category.SalonId,
                category.Name,
                category.Description,
                category.IsActive));
    }
}

public sealed class GetServiceCategoryByIdQueryValidator : AbstractValidator<GetServiceCategoryByIdQuery>
{
    public GetServiceCategoryByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}
