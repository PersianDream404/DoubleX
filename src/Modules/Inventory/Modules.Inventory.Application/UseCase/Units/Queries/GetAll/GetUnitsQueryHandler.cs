using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Inventory.Application.Contract.DTOs.Units;
using Modules.Inventory.Application.Contract.Interfaces.Units;
using Modules.Inventory.Application.Contract.UseCase;

namespace Modules.Inventory.Application.UseCase;

public sealed class GetUnitsQueryHandler(IUnitQueryRepository repository)
    : IQueryHandler<GetUnitsQuery, PagedList<UnitDto>>
{
    public async Task<Result<PagedList<UnitDto>>> Handle(
        GetUnitsQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetAllAsync(query.Request, cancellationToken));
}

public sealed class GetUnitsQueryValidator : AbstractValidator<GetUnitsQuery>
{
    public GetUnitsQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
            RuleFor(query => query.Request.Q).MaximumLength(200));
    }
}
