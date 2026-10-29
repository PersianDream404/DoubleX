using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.Appointment.Application.Contract.DTOs.Appointments;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.Update;
using Modules.Appointment.Domain.Entities;
using Modules.Appointment.Domain.Interfaces;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetAll;

namespace Modules.Appointment.Application.UseCase.Appointments.Commands.Update;

public sealed class UpdateAppointmentCommandHandler(
    IAppointmentCommandRepository repository,
    IQueryBus queryBus)
    : ICommandHandler<UpdateAppointmentCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateAppointmentCommand command,
        CancellationToken cancellationToken)
    {
        var appointment = await repository.GetTrackedAsync(
            command.Id, command.SalonId, cancellationToken);
        if (appointment is null)
            return Result.NotFound("نوبت موردنظر پیدا نشد.");

        var assignmentsResult = await queryBus.Send<
            GetServicePersonnelsQuery,
            IReadOnlyList<ServicePersonnelDto>>(
            new GetServicePersonnelsQuery(command.SalonId, appointment.ServiceId),
            cancellationToken);
        if (!assignmentsResult.IsSuccess ||
            !assignmentsResult.Value.Any(item => item.PersonnelId == command.Request.PersonnelId))
        {
            return Result.Invalid(new ValidationError("پرسنل انتخاب‌شده برای این خدمت مجاز نیست."));
        }

        var personnelResult = await queryBus.Send<
            GetPersonnelByIdQuery,
            GetByIdPersonnelResponseDto>(
            new GetPersonnelByIdQuery(command.Request.PersonnelId),
            cancellationToken);
        if (!personnelResult.IsSuccess ||
            !personnelResult.Value.Status ||
            personnelResult.Value.SalonId != command.SalonId)
        {
            return Result.Invalid(new ValidationError("پرسنل فعال و متعلق به سالن انتخاب نشده است."));
        }

        if (!appointment.UpdateSchedule(
                command.Request.StartsAt,
                command.Request.PersonnelId,
                personnelResult.Value.PersonalInfo.FullName,
                command.Request.Notes,
                DateTimeOffset.UtcNow))
        {
            return Result.Invalid(new ValidationError("ویرایش نوبت با وضعیت فعلی آن مجاز نیست."));
        }

        if (!await repository.UpdateScheduleIfAvailableAsync(appointment, cancellationToken))
            return Result.Invalid(new ValidationError("پرسنل در این بازه زمانی نوبت دیگری دارد."));
        return Result.Success(true);
    }
}

public sealed class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.StartsAt)
                .Must(value => value > DateTimeOffset.UtcNow)
                .WithMessage("زمان نوبت باید در آینده باشد.");
            RuleFor(command => command.Request.PersonnelId).GreaterThan(0);
            RuleFor(command => command.Request.Notes).MaximumLength(2000);
        });
    }
}
