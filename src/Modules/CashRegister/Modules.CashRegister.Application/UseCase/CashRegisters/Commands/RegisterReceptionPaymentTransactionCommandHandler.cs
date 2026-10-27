using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;
using Modules.CashRegister.Domain.Entities;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Commands;

public sealed class RegisterReceptionPaymentTransactionCommandHandler(
    ICashRegisterCommandRepository commandRepository,
    ICashRegisterQueryRepository queryRepository)
    : ICommandHandler<RegisterReceptionPaymentTransactionCommand, bool>
{
    public async Task<Result<bool>> Handle(
        RegisterReceptionPaymentTransactionCommand command,
        CancellationToken cancellationToken)
    {
        var register = await commandRepository.GetRegisterAsync(
            command.CashRegisterId, command.SalonId, cancellationToken);
        if (register is null)
            return Result.NotFound("صندوق متعلق به این سالن پیدا نشد.");

        var session = await commandRepository.GetSessionAsync(command.SessionId, command.SalonId, cancellationToken);
        if (session is null || session.CashRegisterId != command.CashRegisterId)
            return Result.NotFound("جلسهٔ صندوق متعلق به این سالن پیدا نشد.");
        if (!register.Status && session.SessionStatus == CashRegisterSessionStatus.Open)
            return Result.NotFound("صندوق فعال متعلق به این سالن پیدا نشد.");

        var existing = await queryRepository.GetTransactionByIdempotencyKeyAsync(
            command.SalonId, command.IdempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return existing.CashRegisterId == command.CashRegisterId &&
                   existing.SessionId == command.SessionId &&
                   existing.Type == CashRegisterTransactionType.ReceptionPayment &&
                   existing.Amount == command.Amount &&
                   existing.ReferenceId == command.ReceptionId
                ? Result.Success(true)
                : Result.Conflict("کلید پرداخت قبلاً برای گردش دیگری استفاده شده است.");
        }

        if (!session.RegisterTransaction(
                CashRegisterTransactionType.ReceptionPayment,
                command.OccurredAt,
                command.Amount))
            return Result.Conflict("پرداخت پس از بسته‌شدن صندوق انجام شده است.");

        await commandRepository.AddTransactionAsync(
            CashRegisterTransaction.Create(
                command.SalonId,
                command.CashRegisterId,
                session.Id,
                CashRegisterTransactionType.ReceptionPayment,
                command.Amount,
                command.OccurredAt,
                command.UserId,
                command.Description,
                "Reception",
                command.ReceptionId,
                command.IdempotencyKey),
            cancellationToken);

        return await commandRepository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("گردش این پرداخت قبلاً ثبت شده است.");
    }
}

public sealed class RegisterReceptionPaymentTransactionCommandValidator
    : AbstractValidator<RegisterReceptionPaymentTransactionCommand>
{
    public RegisterReceptionPaymentTransactionCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.CashRegisterId).GreaterThan(0);
        RuleFor(command => command.SessionId).GreaterThan(0);
        RuleFor(command => command.ReceptionId).GreaterThan(0);
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Amount).GreaterThan(0).LessThanOrEqualTo(9999999999999999.99m)
            .Must(amount => decimal.Round(amount, 2) == amount);
        RuleFor(command => command.IdempotencyKey).NotEmpty().MaximumLength(100);
        RuleFor(command => command.Description).MaximumLength(1000);
    }
}
