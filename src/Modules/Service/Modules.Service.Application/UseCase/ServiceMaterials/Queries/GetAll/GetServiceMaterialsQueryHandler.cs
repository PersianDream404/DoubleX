using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceMaterials;
using Modules.Service.Application.Contract.Interfaces.ServiceMaterials;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Application.Contract.UseCase.ServiceMaterials.Queries.GetAll;

namespace Modules.Service.Application.UseCase.ServiceMaterials.Queries.GetAll;

public sealed class GetServiceMaterialsQueryHandler(
    IServiceMaterialRepository repository,
    IServiceRepository serviceRepository)
    : IQueryHandler<GetServiceMaterialsQuery, IReadOnlyList<ServiceMaterialDto>>
{
    public async Task<Result<IReadOnlyList<ServiceMaterialDto>>> Handle(
        GetServiceMaterialsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await serviceRepository.ServiceOwnedAsync(query.SalonId, query.ServiceId, cancellationToken))
            return Result.NotFound("خدمت موردنظر در این سالن پیدا نشد.");
        return Result.Success(await repository.GetMaterialsAsync(query.SalonId, query.ServiceId, cancellationToken));
    }
}

public sealed class GetServiceMaterialsQueryValidator : AbstractValidator<GetServiceMaterialsQuery>
{
    public GetServiceMaterialsQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.ServiceId).GreaterThan(0);
    }
}
