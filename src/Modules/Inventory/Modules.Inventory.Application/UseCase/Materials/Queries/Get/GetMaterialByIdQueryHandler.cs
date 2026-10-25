using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.Interfaces.Materials;
using Modules.Inventory.Application.Contract.UseCase;

namespace Modules.Inventory.Application.UseCase;

public sealed class GetMaterialByIdQueryHandler(IMaterialQueryRepository repository)
    : IQueryHandler<GetMaterialByIdQuery, MaterialDto>
{
    public async Task<Result<MaterialDto>> Handle(
        GetMaterialByIdQuery query,
        CancellationToken cancellationToken)
    {
        var material = await repository.GetByIdAsync(query.Id, cancellationToken);
        return material is null
            ? Result.NotFound("کالای موردنظر پیدا نشد.")
            : Result.Success(material);
    }
}

public sealed class GetMaterialByIdQueryValidator : AbstractValidator<GetMaterialByIdQuery>
{
    public GetMaterialByIdQueryValidator() =>
        RuleFor(query => query.Id).GreaterThan(0);
}
