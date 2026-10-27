using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;
using Modules.CashRegister.Domain.Entities;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Commands;

public sealed class OpenCashRegisterSessionCommandHandler(ICashRegisterCommandRepository repository)
    : ICommandHandler<OpenCashRegisterSessionCommand, long>
{
    public async Task<Result<long>> Handle(OpenCashRegisterSessionCommand command, CancellationToken cancellationToken)
    {
        var register = await repository.GetRegisterAsync(command.CashRegisterId, command.SalonId, cancellationToken);
        if (register is null)
            return Result.NotFound("صندوق موردنظر پیدا نشد.");
        if (!register.Status)
            return Result.Error("صندوق غیرفعال است و امکان افتتاح ندارد.");
        if (await repository.GetOpenSessionAsync(command.CashRegisterId, command.SalonId, cancellationToken) is not null)
            return Result.Conflict("برای این صندوق یک جلسهٔ باز وجود دارد.");

        var session = CashRegisterSession.Open(
            command.SalonId,
            command.CashRegisterId,
            command.UserId,
            DateTimeOffset.UtcNow,
            command.OpeningBalance);
        session.AddOpeningBalanceTransaction(command.UserId);
        return await repository.AddSessionWithOpeningTransactionAsync(session, cancellationToken)
            ? Result.Success(session.Id)
            : Result.Conflict("افتتاح صندوق هم‌زمان انجام شده است؛ وضعیت صندوق را بررسی کنید.");
    }
}

public sealed class OpenCashRegisterSessionCommandValidator : AbstractValidator<OpenCashRegisterSessionCommand>
{
    public OpenCashRegisterSessionCommandValidator()
    {
        RuleFor(command => command.CashRegisterId).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.OpeningBalance).InclusiveBetween(0, 9999999999999999.99m)
            .Must(amount => decimal.Round(amount, 2) == amount);
    }
}
