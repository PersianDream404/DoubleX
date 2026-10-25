using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.Interfaces.Materials;
using Modules.Inventory.Application.Contract.UseCase;

namespace Modules.Inventory.Application.UseCase;

public sealed class GetMaterialsQueryHandler(IMaterialQueryRepository repository)
    : IQueryHandler<GetMaterialsQuery, PagedList<MaterialDto>>
{
    public async Task<Result<PagedList<MaterialDto>>> Handle(
        GetMaterialsQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetAllAsync(query.Request, cancellationToken));
}

public sealed class GetMaterialsQueryValidator : AbstractValidator<GetMaterialsQuery>
{
    public GetMaterialsQueryValidator()
    {
        RuleFor(query => query.Request).NotNull();
        When(query => query.Request is not null, () =>
            RuleFor(query => query.Request.Q).MaximumLength(250));
    }
}
