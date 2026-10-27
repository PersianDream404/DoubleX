using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;
using Modules.CashRegister.Domain.Entities;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Commands;

public sealed class RecordCashMovementCommandHandler(ICashRegisterCommandRepository repository)
    : ICommandHandler<RecordCashMovementCommand, bool>
{
    public async Task<Result<bool>> Handle(RecordCashMovementCommand command, CancellationToken cancellationToken)
    {
        var register = await repository.GetRegisterAsync(command.CashRegisterId, command.SalonId, cancellationToken);
        if (register is null)
            return Result.NotFound("صندوق موردنظر پیدا نشد.");
        if (!register.Status)
            return Result.Error("صندوق غیرفعال است.");

        var session = await repository.GetOpenSessionAsync(command.CashRegisterId, command.SalonId, cancellationToken);
        if (session is null)
            return Result.Error("برای ثبت گردش، صندوق باید جلسهٔ باز داشته باشد.");

        var existing = await repository.IsTransactionIdempotencyKeyUsedAsync(
            command.SalonId,
            command.IdempotencyKey,
            cancellationToken);
        if (existing)
            return Result.Conflict("این کلید ثبت قبلاً استفاده شده است.");

        if (command.Type == CashRegisterTransactionType.CashOut &&
            command.Amount > await repository.GetExpectedBalanceAsync(session.Id, cancellationToken))
        {
            return Result.Error("مبلغ خروجی از موجودی مورد انتظار صندوق بیشتر است.");
        }

        var occurredAt = DateTimeOffset.UtcNow;
        if (!session.RegisterTransaction(command.Type, occurredAt, command.Amount))
            return Result.Conflict("جلسهٔ صندوق بسته است.");

        await repository.AddTransactionAsync(
            CashRegisterTransaction.Create(
                command.SalonId,
                command.CashRegisterId,
                session.Id,
                command.Type,
                command.Amount,
                occurredAt,
                command.UserId,
                command.Description,
                null,
                null,
                command.IdempotencyKey),
            cancellationToken);
        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("گردش تکراری ثبت نشد؛ وضعیت صندوق را بررسی کنید.");
    }
}

public sealed class RecordCashMovementCommandValidator : AbstractValidator<RecordCashMovementCommand>
{
    public RecordCashMovementCommandValidator()
    {
        RuleFor(command => command.CashRegisterId).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Type)
            .Must(type => type is CashRegisterTransactionType.CashIn or CashRegisterTransactionType.CashOut);
        RuleFor(command => command.Amount).GreaterThan(0).LessThanOrEqualTo(9999999999999999.99m)
            .Must(amount => decimal.Round(amount, 2) == amount);
        RuleFor(command => command.IdempotencyKey).NotEmpty().MaximumLength(100);
        RuleFor(command => command.Description).NotEmpty().MaximumLength(1000);
    }
}
