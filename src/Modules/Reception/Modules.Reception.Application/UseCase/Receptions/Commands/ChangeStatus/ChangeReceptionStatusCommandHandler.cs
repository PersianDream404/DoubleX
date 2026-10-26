using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.UseCase.Receptions.Commands.ChangeStatus;

namespace Modules.Reception.Application.UseCase.Receptions.Commands.ChangeStatus;

public sealed class ChangeReceptionStatusCommandHandler(IReceptionCommandRepository repository)
    : ICommandHandler<ChangeReceptionStatusCommand, bool>
{
    public async Task<Result<bool>> Handle(
        ChangeReceptionStatusCommand command,
        CancellationToken cancellationToken)
    {
        var reception = await repository.GetTrackedAsync(
            command.Id,
            command.SalonId,
            cancellationToken);
        if (reception is null)
        {
            return Result.NotFound("پذیرش موردنظر پیدا نشد.");
        }

        if (!reception.ChangeStatus(command.Status))
        {
            return Result.Error(
                "تغییر وضعیت مجاز نیست؛ پذیرش مصرف انبار ثبت‌شده دارد یا در وضعیت نهایی است.");
        }

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("پذیرش هم‌زمان تغییر کرده است؛ دوباره اطلاعات را بررسی کنید.");
    }
}

public sealed class ChangeReceptionStatusCommandValidator : AbstractValidator<ChangeReceptionStatusCommand>
{
    public ChangeReceptionStatusCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.Status).IsInEnum();
    }
}
