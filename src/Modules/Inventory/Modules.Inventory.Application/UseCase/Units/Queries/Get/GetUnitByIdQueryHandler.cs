using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Inventory.Application.Contract.DTOs.Units;
using Modules.Inventory.Application.Contract.Interfaces.Units;
using Modules.Inventory.Application.Contract.UseCase;

namespace Modules.Inventory.Application.UseCase;

public sealed class GetUnitByIdQueryHandler(IUnitQueryRepository repository)
    : IQueryHandler<GetUnitByIdQuery, UnitDto>
{
    public async Task<Result<UnitDto>> Handle(
        GetUnitByIdQuery query,
        CancellationToken cancellationToken)
    {
        var unit = await repository.GetByIdAsync(query.Id, cancellationToken);
        return unit is null
            ? Result.NotFound("واحد موردنظر پیدا نشد.")
            : Result.Success(unit);
    }
}

public sealed class GetUnitByIdQueryValidator : AbstractValidator<GetUnitByIdQuery>
{
    public GetUnitByIdQueryValidator() =>
        RuleFor(query => query.Id).GreaterThan(0);
}
