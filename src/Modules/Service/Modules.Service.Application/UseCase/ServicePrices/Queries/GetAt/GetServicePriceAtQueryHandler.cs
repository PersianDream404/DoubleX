using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePrices;
using Modules.Service.Application.Contract.Interfaces.ServicePrices;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Application.Contract.UseCase.ServicePrices.Queries.GetAt;

namespace Modules.Service.Application.UseCase.ServicePrices.Queries.GetAt;

public sealed class GetServicePriceAtQueryHandler(
    IServicePriceRepository repository,
    IServiceRepository serviceRepository)
    : IQueryHandler<GetServicePriceAtQuery, ServicePriceDto>
{
    public async Task<Result<ServicePriceDto>> Handle(
        GetServicePriceAtQuery query,
        CancellationToken cancellationToken)
    {
        if (query.At == default)
            return Result.Invalid(new ValidationError("تاریخ بررسی تعرفه الزامی است."));
        if (!await serviceRepository.ServiceOwnedAsync(query.SalonId, query.ServiceId, cancellationToken))
            return Result.NotFound("خدمت موردنظر در این سالن پیدا نشد.");
        var price = await repository.GetPriceAtAsync(query.SalonId, query.ServiceId, query.At, cancellationToken);
        return price is null
            ? Result.NotFound("برای این تاریخ تعرفه معتبری ثبت نشده است.")
            : Result.Success(price);
    }
}

public sealed class GetServicePriceAtQueryValidator : AbstractValidator<GetServicePriceAtQuery>
{
    public GetServicePriceAtQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.ServiceId).GreaterThan(0);
        RuleFor(query => query.At).NotEmpty();
    }
}
