using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Appointment.Application.Contract.UseCase.Appointments.Commands.ChangeStatus;
using Modules.Appointment.Domain.Entities;
using Modules.Appointment.Domain.Interfaces;

namespace Modules.Appointment.Application.UseCase.Appointments.Commands.ChangeStatus;

public sealed class ChangeAppointmentStatusCommandHandler(
    IAppointmentCommandRepository repository)
    : ICommandHandler<ChangeAppointmentStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(
        ChangeAppointmentStatusCommand command,
        CancellationToken cancellationToken)
    {
        var appointment = await repository.GetTrackedAsync(
            command.Id, command.SalonId, cancellationToken);
        if (appointment is null)
            return Result.NotFound("نوبت موردنظر پیدا نشد.");

        if (!appointment.ChangeStatus(command.Status, DateTimeOffset.UtcNow))
            return Result.Invalid(new ValidationError("تغییر وضعیت نوبت با وضعیت فعلی آن مجاز نیست."));

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("نوبت هم‌زمان توسط کاربر دیگری تغییر کرده است.");
    }
}

public sealed class ChangeAppointmentStatusCommandValidator
    : AbstractValidator<ChangeAppointmentStatusCommand>
{
    public ChangeAppointmentStatusCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.Status)
            .Must(status => status is AppointmentStatus.Confirmed or
                AppointmentStatus.Cancelled or AppointmentStatus.NoShow)
            .WithMessage("وضعیت درخواستی از این عملیات پشتیبانی نمی‌شود.");
    }
}
