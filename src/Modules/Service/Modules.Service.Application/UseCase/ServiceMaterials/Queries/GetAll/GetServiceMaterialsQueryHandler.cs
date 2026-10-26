using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceMaterials;
using Modules.Service.Application.Contract.UseCase.ServiceMaterials.Queries.GetAll;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.ServiceMaterials.Queries.GetAll;

public sealed class GetServiceMaterialsQueryHandler(
    IServiceMaterialQueryRepository repository,
    IServiceQueryRepository serviceRepository)
    : IQueryHandler<GetServiceMaterialsQuery, IReadOnlyList<ServiceMaterialDto>>
{
    public async Task<Result<IReadOnlyList<ServiceMaterialDto>>> Handle(
        GetServiceMaterialsQuery query,
        CancellationToken cancellationToken)
    {
        if (!await serviceRepository.ServiceOwnedAsync(query.SalonId, query.ServiceId, cancellationToken))
            return Result.NotFound("خدمت موردنظر در این سالن پیدا نشد.");
        var materials = await repository.GetMaterialsAsync(
            query.SalonId,
            query.ServiceId,
            cancellationToken);
        return Result.Success<IReadOnlyList<ServiceMaterialDto>>(materials
            .Select(material => new ServiceMaterialDto(
                material.Id,
                material.SalonId,
                material.ServiceId,
                material.MaterialId,
                material.MaterialName,
                material.BaseUnitName,
                material.BaseUnitSymbol,
                material.QuantityPerService,
                material.Description))
            .ToList());
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
