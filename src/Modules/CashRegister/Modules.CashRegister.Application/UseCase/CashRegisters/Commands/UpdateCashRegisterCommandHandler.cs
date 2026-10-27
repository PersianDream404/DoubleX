using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Commands;

public sealed class UpdateCashRegisterCommandHandler(ICashRegisterCommandRepository repository)
    : ICommandHandler<UpdateCashRegisterCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateCashRegisterCommand command, CancellationToken cancellationToken)
    {
        var register = await repository.GetRegisterAsync(command.Id, command.SalonId, cancellationToken);
        if (register is null)
            return Result.NotFound("صندوق موردنظر پیدا نشد.");
        if (await repository.IsCodeUsedAsync(
                command.SalonId,
                command.Request.Code.Trim(),
                command.Id,
                cancellationToken))
        {
            return Result.Conflict("کد صندوق در این سالن قبلاً ثبت شده است.");
        }

        register.Rename(command.Request.Name, command.Request.Code, command.Request.Description);
        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("صندوق هم‌زمان تغییر کرده یا کد آن تکراری است.");
    }
}

public sealed class UpdateCashRegisterCommandValidator : AbstractValidator<UpdateCashRegisterCommand>
{
    public UpdateCashRegisterCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.Name).NotEmpty().MaximumLength(150);
            RuleFor(command => command.Request.Code).NotEmpty().MaximumLength(50)
                .Matches("^[A-Za-z0-9_-]+$");
            RuleFor(command => command.Request.Description).MaximumLength(1000);
        });
    }
}
