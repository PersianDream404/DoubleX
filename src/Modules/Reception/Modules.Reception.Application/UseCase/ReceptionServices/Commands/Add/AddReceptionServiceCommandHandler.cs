using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.UseCase.ReceptionServices.Commands.Add;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetById;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetById;

namespace Modules.Reception.Application.UseCase.ReceptionServices.Commands.Add;

public sealed class AddReceptionServiceCommandHandler(
    IReceptionCommandRepository repository,
    IQueryBus queryBus)
    : ICommandHandler<AddReceptionServiceCommand, bool>
{
    public async Task<Result<bool>> Handle(
        AddReceptionServiceCommand command,
        CancellationToken cancellationToken)
    {
        var reception = await repository.GetTrackedAsync(
            command.ReceptionId,
            command.Request.SalonId,
            cancellationToken);
        if (reception is null)
        {
            return Result.NotFound("پذیرش موردنظر پیدا نشد.");
        }

        var tariffResult = await queryBus.Send<GetServiceTariffByIdQuery, ServiceTariffDto>(
            new GetServiceTariffByIdQuery(command.Request.ServiceTariffId),
            cancellationToken);
        if (!tariffResult.IsSuccess ||
            tariffResult.Value.SalonId != command.Request.SalonId ||
            tariffResult.Value.ValidFrom > DateTimeOffset.UtcNow ||
            (tariffResult.Value.ValidTo.HasValue &&
             tariffResult.Value.ValidTo.Value < DateTimeOffset.UtcNow))
        {
            return Result.Invalid(new ValidationError("تعرفهٔ معتبر و متعلق به سالن انتخاب‌شده پیدا نشد."));
        }

        var serviceResult = await queryBus.Send<GetServiceByIdQuery, ServiceDto>(
            new GetServiceByIdQuery(command.Request.ServiceId),
            cancellationToken);
        if (!serviceResult.IsSuccess ||
            !serviceResult.Value.IsActive ||
            serviceResult.Value.SalonId != command.Request.SalonId)
        {
            return Result.Invalid(new ValidationError("خدمت فعال و متعلق به سالن انتخاب‌شده نیست."));
        }

        var tariffItem = tariffResult.Value.Services
            .SingleOrDefault(item => item.ServiceId == command.Request.ServiceId);
        if (tariffItem is null)
        {
            return Result.Invalid(new ValidationError("خدمت در تعرفهٔ انتخاب‌شده تعریف نشده است."));
        }

        if (command.Request.PersonnelId.HasValue)
        {
            var personnelResult = await queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>(
                new GetPersonnelByIdQuery(command.Request.PersonnelId.Value),
                cancellationToken);
            if (!personnelResult.IsSuccess ||
                !personnelResult.Value.Status ||
                personnelResult.Value.SalonId != command.Request.SalonId)
            {
                return Result.Invalid(new ValidationError("پرسنل فعال و متعلق به سالن انتخاب‌شده نیست."));
            }
        }

        if (!reception.TryAddService(
                command.Request.ServiceId,
                command.Request.ServiceTariffId,
                serviceResult.Value.Name,
                command.Request.PersonnelId,
                command.Request.Quantity,
                tariffItem.Amount,
                command.Request.DiscountAmount))
        {
            return Result.Error("افزودن خدمت به پذیرش در وضعیت فعلی مجاز نیست.");
        }

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("پذیرش هم‌زمان تغییر کرده است؛ دوباره اطلاعات را بررسی کنید.");
    }
}

public sealed class AddReceptionServiceCommandValidator : AbstractValidator<AddReceptionServiceCommand>
{
    public AddReceptionServiceCommandValidator()
    {
        RuleFor(command => command.ReceptionId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.SalonId).GreaterThan(0);
            RuleFor(command => command.Request.ServiceId).GreaterThan(0);
            RuleFor(command => command.Request.ServiceTariffId).GreaterThan(0);
            RuleFor(command => command.Request.PersonnelId).GreaterThan(0).When(command =>
                command.Request.PersonnelId.HasValue);
            RuleFor(command => command.Request.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(1000000000m);
            RuleFor(command => command.Request.DiscountAmount).GreaterThanOrEqualTo(0);
        });
    }
}
