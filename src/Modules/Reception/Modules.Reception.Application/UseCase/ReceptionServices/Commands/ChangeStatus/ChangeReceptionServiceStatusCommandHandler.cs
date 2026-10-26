using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.UseCase.ReceptionServices.Commands.ChangeStatus;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.UseCase.ReceptionServices.Commands.ChangeStatus;

public sealed class ChangeReceptionServiceStatusCommandHandler(IReceptionCommandRepository repository)
    : ICommandHandler<ChangeReceptionServiceStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(
        ChangeReceptionServiceStatusCommand command,
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

        if (reception.ReceptionStatus is ReceptionStatus.Completed or ReceptionStatus.Cancelled ||
            !reception.ChangeServiceStatus(command.ServiceId, command.Request.Status))
        {
            return Result.Error("تغییر وضعیت خدمت در وضعیت فعلی مجاز نیست.");
        }

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("پذیرش هم‌زمان تغییر کرده است.");
    }
}

public sealed class ChangeReceptionServiceStatusCommandValidator
    : AbstractValidator<ChangeReceptionServiceStatusCommand>
{
    public ChangeReceptionServiceStatusCommandValidator()
    {
        RuleFor(command => command.ReceptionId).GreaterThan(0);
        RuleFor(command => command.ServiceId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.SalonId).GreaterThan(0);
            RuleFor(command => command.Request.Status).IsInEnum();
        });
    }
}
