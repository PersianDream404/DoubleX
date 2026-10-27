using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Commands;

public sealed class CloseCashRegisterSessionCommandHandler(ICashRegisterCommandRepository repository)
    : ICommandHandler<CloseCashRegisterSessionCommand, bool>
{
    public async Task<Result<bool>> Handle(CloseCashRegisterSessionCommand command, CancellationToken cancellationToken)
    {
        var session = await repository.GetSessionAsync(command.SessionId, command.SalonId, cancellationToken);
        if (session is null || session.CashRegisterId != command.CashRegisterId)
            return Result.NotFound("جلسهٔ صندوق موردنظر پیدا نشد.");

        var expectedBalance = await repository.GetExpectedBalanceAsync(session.Id, cancellationToken);
        if (!session.Close(command.UserId, DateTimeOffset.UtcNow, expectedBalance, command.ActualClosingBalance))
            return Result.Conflict("جلسهٔ صندوق قبلاً بسته شده یا مبلغ شمارش‌شده معتبر نیست.");

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("جلسهٔ صندوق هم‌زمان تغییر کرده است.");
    }
}

public sealed class CloseCashRegisterSessionCommandValidator : AbstractValidator<CloseCashRegisterSessionCommand>
{
    public CloseCashRegisterSessionCommandValidator()
    {
        RuleFor(command => command.CashRegisterId).GreaterThan(0);
        RuleFor(command => command.SessionId).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.ActualClosingBalance).InclusiveBetween(0, 9999999999999999.99m)
            .Must(amount => decimal.Round(amount, 2) == amount);
    }
}
