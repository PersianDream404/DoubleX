using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetById;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.ServicePersonnels.Queries.GetById;

public sealed class GetServicePersonnelByIdQueryHandler(
    IServicePersonnelQueryRepository repository,
    IServiceQueryRepository serviceRepository)
    : IQueryHandler<GetServicePersonnelByIdQuery, ServicePersonnelDto>
{
    public async Task<Result<ServicePersonnelDto>> Handle(
        GetServicePersonnelByIdQuery query,
        CancellationToken cancellationToken)
    {
        if (!await serviceRepository.ServiceOwnedAsync(
                query.SalonId,
                query.ServiceId,
                cancellationToken))
        {
            return Result.NotFound("خدمت موردنظر در این سالن پیدا نشد.");
        }

        var item = await repository.GetByIdAsync(
            query.SalonId,
            query.ServiceId,
            query.Id,
            cancellationToken);
        return item is null
            ? Result.NotFound("پرسنل ثبت‌شده برای خدمت پیدا نشد.")
            : Result.Success(new ServicePersonnelDto(
                item.Id,
                item.SalonId,
                item.ServiceId,
                item.PersonnelId));
    }
}

public sealed class GetServicePersonnelByIdQueryValidator
    : AbstractValidator<GetServicePersonnelByIdQuery>
{
    public GetServicePersonnelByIdQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.ServiceId).GreaterThan(0);
        RuleFor(query => query.Id).GreaterThan(0);
    }
}
