using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.Get;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.UseCase.Receptions.Commands.Create;
using Modules.Reception.Domain.Entities;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetById;
using ReceptionEntity = Modules.Reception.Domain.Entities.Reception;

namespace Modules.Reception.Application.UseCase.Receptions.Commands.Create;

public sealed class CreateReceptionCommandHandler(
    IReceptionCommandRepository repository,
    IQueryBus queryBus)
    : ICommandHandler<CreateReceptionCommand, long>
{
    public async Task<Result<long>> Handle(
        CreateReceptionCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        if (request.AppointmentId.HasValue)
        {
            return Result.Error("اتصال پذیرش به نوبت پشتیبانی نمی‌شود؛ ماژول Appointment در سامانه موجود نیست.");
        }

        var tariffResult = await queryBus.Send<GetServiceTariffByIdQuery, ServiceTariffDto>(
            new GetServiceTariffByIdQuery(request.ServiceTariffId),
            cancellationToken);
        if (!tariffResult.IsSuccess ||
            tariffResult.Value.SalonId != request.SalonId ||
            tariffResult.Value.ValidFrom > DateTimeOffset.UtcNow ||
            (tariffResult.Value.ValidTo.HasValue &&
             tariffResult.Value.ValidTo.Value < DateTimeOffset.UtcNow))
        {
            return Result.Invalid(new ValidationError("تعرفهٔ معتبر و متعلق به سالن انتخاب‌شده پیدا نشد."));
        }

        var customerResult = await queryBus.Send<GetCustomerByIdQuery, CustomerDetailsDto>(
            new GetCustomerByIdQuery(request.CustomerId),
            cancellationToken);
        if (!customerResult.IsSuccess ||
            !customerResult.Value.Status ||
            customerResult.Value.SalonId != request.SalonId)
        {
            return Result.Invalid(new ValidationError("مشتری فعال و متعلق به سالن انتخاب‌شده نیست."));
        }

        var receptionistResult = await queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>(
            new GetPersonnelByIdQuery(request.ReceptionistId),
            cancellationToken);
        if (!receptionistResult.IsSuccess ||
            !receptionistResult.Value.Status ||
            receptionistResult.Value.SalonId != request.SalonId)
        {
            return Result.Invalid(new ValidationError("پذیرشگر فعال و متعلق به سالن انتخاب‌شده نیست."));
        }

        var reception = ReceptionEntity.Create(
            request.SalonId,
            request.CustomerId,
            customerResult.Value.FullName,
            request.ReceptionistId,
            request.ServiceTariffId,
            request.ReceptionDate,
            request.Notes);
        await repository.AddAsync(reception, cancellationToken);
        return Result.Success(reception.Id);
    }
}

public sealed class CreateReceptionCommandValidator : AbstractValidator<CreateReceptionCommand>
{
    public CreateReceptionCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.SalonId).GreaterThan(0);
            RuleFor(command => command.Request.CustomerId).GreaterThan(0);
            RuleFor(command => command.Request.ReceptionistId).GreaterThan(0);
            RuleFor(command => command.Request.ServiceTariffId).GreaterThan(0);
            RuleFor(command => command.Request.ReceptionDate).NotEmpty();
            RuleFor(command => command.Request.Notes).MaximumLength(2000);
            RuleFor(command => command.Request.AppointmentId)
                .GreaterThan(0)
                .When(command => command.Request.AppointmentId.HasValue);
        });
    }
}
