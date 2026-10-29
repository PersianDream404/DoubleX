using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.Create;
using Modules.Appointment.Domain.Interfaces;
using AppointmentEntity = Modules.Appointment.Domain.Entities.Appointment;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.Get;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetAll;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetById;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetActive;

namespace Modules.Appointment.Application.UseCase.Appointments.Commands.Create;

public sealed class CreateAppointmentCommandHandler(
    IAppointmentCommandRepository repository,
    IQueryBus queryBus)
    : ICommandHandler<CreateAppointmentCommand, long>
{
    public async Task<Result<long>> Handle(
        CreateAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var salonId = command.SalonId;
        var customerResult = await queryBus.Send<GetCustomerByIdQuery, CustomerDetailsDto>(
            new GetCustomerByIdQuery(request.CustomerId), cancellationToken);
        if (!customerResult.IsSuccess ||
            !customerResult.Value.Status ||
            customerResult.Value.SalonId <= 0)
        {
            return Result.Invalid(new ValidationError("مشتری فعال و معتبری انتخاب نشده است."));
        }

        var serviceResult = await queryBus.Send<GetActiveServicesQuery, IReadOnlyList<ServiceDto>>(
            new GetActiveServicesQuery(salonId),
            cancellationToken);
        if (!serviceResult.IsSuccess)
            return Result.Invalid(new ValidationError("دریافت خدمات فعال سالن ناموفق بود."));
        var service = serviceResult.Value.SingleOrDefault(item =>
            item.Id == request.ServiceId && item.SalonId == salonId && item.IsActive);
        if (service is null)
            return Result.Invalid(new ValidationError("خدمت فعال و متعلق به سالن انتخاب نشده است."));

        var tariffResult = await queryBus.Send<GetServiceTariffByIdQuery, ServiceTariffDto>(
            new GetServiceTariffByIdQuery(request.ServiceTariffId),
            cancellationToken);
        if (!tariffResult.IsSuccess ||
            tariffResult.Value.SalonId != salonId ||
            tariffResult.Value.ValidFrom > request.StartsAt ||
            (tariffResult.Value.ValidTo.HasValue &&
             tariffResult.Value.ValidTo.Value < request.StartsAt))
        {
            return Result.Invalid(new ValidationError("تعرفه در تاریخ نوبت معتبر نیست."));
        }
        var tariffItem = tariffResult.Value.Services.SingleOrDefault(item =>
            item.ServiceId == request.ServiceId);
        if (tariffItem is null || tariffItem.Amount < 0)
            return Result.Invalid(new ValidationError("خدمت در تعرفه انتخاب‌شده وجود ندارد."));

        var assignmentsResult = await queryBus.Send<
            GetServicePersonnelsQuery,
            IReadOnlyList<ServicePersonnelDto>>(
            new GetServicePersonnelsQuery(salonId, request.ServiceId),
            cancellationToken);
        if (!assignmentsResult.IsSuccess ||
            !assignmentsResult.Value.Any(item => item.PersonnelId == request.PersonnelId))
        {
            return Result.Invalid(new ValidationError("پرسنل انتخاب‌شده برای این خدمت مجاز نیست."));
        }

        var personnelResult = await queryBus.Send<
            GetPersonnelByIdQuery,
            GetByIdPersonnelResponseDto>(
            new GetPersonnelByIdQuery(request.PersonnelId),
            cancellationToken);
        if (!personnelResult.IsSuccess ||
            !personnelResult.Value.Status ||
            personnelResult.Value.SalonId != salonId)
        {
            return Result.Invalid(new ValidationError("پرسنل فعال و متعلق به سالن انتخاب نشده است."));
        }

        if (customerResult.Value.SalonId != salonId)
            return Result.Invalid(new ValidationError("مشتری متعلق به سالن دیگری است."));
        if (request.DiscountAmount < 0 || request.DiscountAmount > tariffItem.Amount)
            return Result.Invalid(new ValidationError("مبلغ تخفیف باید بین صفر و مبلغ خدمت باشد."));

        var appointment = AppointmentEntity.Create(
            salonId,
            customerResult.Value.Id,
            customerResult.Value.FullName,
            service.Id,
            service.Name,
            tariffResult.Value.Id,
            request.PersonnelId,
            personnelResult.Value.PersonalInfo.FullName,
            request.StartsAt,
            service.DurationMinutes,
            request.Notes,
            tariffItem.Amount,
            request.DiscountAmount,
            request.NotificationChannel,
            request.IdempotencyKey,
            DateTimeOffset.UtcNow);

        var writeResult = await repository.AddIfAvailableAsync(appointment, cancellationToken);
        return writeResult switch
        {
            AppointmentWriteResult.Created => Result.Success(appointment.Id),
            AppointmentWriteResult.ExistingRequest => await GetExistingAppointmentIdAsync(
                repository,
                salonId,
                request.IdempotencyKey,
                cancellationToken),
            AppointmentWriteResult.IdempotencyKeyReused =>
                Result.Invalid(new ValidationError("کلید درخواست قبلاً برای اطلاعات دیگری استفاده شده است.")),
            _ => Result.Invalid(new ValidationError("پرسنل در این بازه زمانی نوبت دیگری دارد."))
        };
    }

    private static async Task<Result<long>> GetExistingAppointmentIdAsync(
        IAppointmentCommandRepository repository,
        long salonId,
        Guid idempotencyKey,
        CancellationToken cancellationToken)
    {
        var existing = await repository.GetByIdempotencyKeyAsync(
            salonId,
            idempotencyKey,
            cancellationToken);
        return existing is null
            ? Result.Conflict("نوبت در زمان پردازش درخواست تغییر کرده است؛ دوباره تلاش کنید.")
            : Result.Success(existing.Id);
    }
}

public sealed class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.SalonId).GreaterThan(0);
            RuleFor(command => command.Request.IdempotencyKey).NotEmpty();
            RuleFor(command => command.Request.CustomerId).GreaterThan(0);
            RuleFor(command => command.Request.ServiceId).GreaterThan(0);
            RuleFor(command => command.Request.ServiceTariffId).GreaterThan(0);
            RuleFor(command => command.Request.PersonnelId).GreaterThan(0);
            RuleFor(command => command.Request.StartsAt)
                .Must(value => value > DateTimeOffset.UtcNow)
                .WithMessage("زمان نوبت باید در آینده باشد.");
            RuleFor(command => command.Request.DiscountAmount)
                .GreaterThanOrEqualTo(0)
                .Must(value => value == decimal.Round(value, 2))
                .WithMessage("مبلغ تخفیف باید حداکثر دو رقم اعشار داشته باشد.");
            RuleFor(command => command.Request.Notes).MaximumLength(2000);
            RuleFor(command => command.Request.NotificationChannel)
                .IsInEnum();
        });
    }
}
