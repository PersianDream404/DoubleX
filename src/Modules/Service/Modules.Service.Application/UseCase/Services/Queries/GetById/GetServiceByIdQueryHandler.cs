using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetById;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.Services.Queries.GetById;

public sealed class GetServiceByIdQueryHandler(IServiceQueryRepository repository)
    : IQueryHandler<GetServiceByIdQuery, ServiceDto>
{
    public async Task<Result<ServiceDto>> Handle(
        GetServiceByIdQuery query,
        CancellationToken cancellationToken)
    {
        var service = await repository.GetServiceByIdAsync(query.Id, cancellationToken);
        return service is null
            ? Result.NotFound("خدمت موردنظر پیدا نشد.")
            : Result.Success(new ServiceDto(
                service.Id,
                service.SalonId,
                service.CategoryId,
                service.Code,
                service.Name,
                service.Description,
                service.DurationMinutes,
                service.IsActive));
    }
}

public sealed class GetServiceByIdQueryValidator : AbstractValidator<GetServiceByIdQuery>
{
    public GetServiceByIdQueryValidator() => RuleFor(query => query.Id).GreaterThan(0);
}
