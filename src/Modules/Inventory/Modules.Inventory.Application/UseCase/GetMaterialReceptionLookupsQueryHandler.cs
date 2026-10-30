using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.Interfaces.Materials;
using Modules.Inventory.Application.Contract.UseCase;

namespace Modules.Inventory.Application.UseCase;

public sealed class GetMaterialReceptionLookupsQueryHandler(
    IMaterialQueryRepository repository)
    : IQueryHandler<GetMaterialReceptionLookupsQuery, IReadOnlyList<MaterialReceptionLookupDto>>
{
    public async Task<Result<IReadOnlyList<MaterialReceptionLookupDto>>> Handle(
        GetMaterialReceptionLookupsQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetReceptionLookupsAsync(
            query.MaterialIds,
            cancellationToken));
}

public sealed class GetMaterialReceptionLookupsQueryValidator
    : AbstractValidator<GetMaterialReceptionLookupsQuery>
{
    public GetMaterialReceptionLookupsQueryValidator()
    {
        RuleFor(query => query.MaterialIds)
            .NotNull()
            .Must(ids => ids.Count <= 501)
            .WithMessage("تعداد شناسه‌های مواد بیش از حد مجاز است.");
        When(query => query.MaterialIds is not null, () =>
            RuleForEach(query => query.MaterialIds).GreaterThan(0));
    }
}
