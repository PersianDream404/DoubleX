using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;
using Modules.CashRegister.Domain.Entities;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.DTOs.Payments.Register;
using Modules.Reception.Application.Contract.UseCase.Payments.Commands.Register;
using Modules.Reception.Domain.Entities;
using System.Security.Cryptography;
using System.Text;

namespace Modules.Reception.Application.UseCase.Payments.Commands.Register;

public sealed class RegisterReceptionPaymentCommandHandler(
    IReceptionCommandRepository repository,
    IQueryBus queryBus)
    : ICommandHandler<RegisterReceptionPaymentCommand, bool>
{
    public async Task<Result<bool>> Handle(
        RegisterReceptionPaymentCommand command,
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

        var existingPayment = reception.Payments.SingleOrDefault(
            payment => payment.IdempotencyKey == command.Request.IdempotencyKey);
        if (existingPayment is not null)
        {
            return existingPayment.Amount == command.Request.Amount &&
                   existingPayment.PaymentMethod == command.Request.PaymentMethod.Trim() &&
                   existingPayment.CashRegisterId == command.Request.CashRegisterId &&
                   existingPayment.TransactionReference == command.Request.TransactionReference?.Trim()
                ? Result.Success(true)
                : Result.Conflict("کلید پرداخت قبلاً برای اطلاعات دیگری استفاده شده است.");
        }

        var isCash = string.Equals(
            command.Request.PaymentMethod.Trim(),
            "نقدی",
            StringComparison.OrdinalIgnoreCase);
        long? sessionId = null;
        var acceptedAt = DateTimeOffset.UtcNow;
        if (isCash)
        {
            if (!command.Request.CashRegisterId.HasValue)
                return Result.Error("برای پرداخت نقدی انتخاب صندوق الزامی است.");

            var activeSession = await queryBus.Send<
                GetCashRegisterSessionQuery,
                CashRegisterSessionDto>(
                new GetCashRegisterSessionQuery(
                    command.Request.CashRegisterId.Value,
                    command.Request.SalonId),
                cancellationToken);
            if (!activeSession.IsSuccess ||
                activeSession.Value!.CashRegisterId != command.Request.CashRegisterId.Value ||
                activeSession.Value.Status != CashRegisterSessionStatus.Open)
            {
                return Result.Error("صندوق انتخاب‌شده جلسهٔ باز و فعال ندارد.");
            }

            sessionId = activeSession.Value.Id;
        }
        else if (command.Request.CashRegisterId.HasValue)
        {
            return Result.Error("برای روش پرداخت غیرنقدی نباید صندوق نقدی انتخاب شود.");
        }

        if (!reception.TryAddPayment(
                command.Request.Amount,
                command.Request.PaymentMethod,
                command.Request.PaidAt,
                command.Request.TransactionReference,
                command.Request.IdempotencyKey,
                command.Request.Description,
                command.Request.CashRegisterId))
        {
            return Result.Error("پرداخت تکراری، نامعتبر یا بیشتر از ماندهٔ همین پذیرش است.");
        }

        if (sessionId.HasValue)
        {
            var idempotencyKey = Convert.ToHexString(SHA256.HashData(
                Encoding.UTF8.GetBytes($"{reception.Id}:{command.Request.IdempotencyKey}")));
            repository.AddCashRegisterPaymentOutbox(CashRegisterPaymentOutbox.Create(
                command.Request.SalonId,
                command.Request.CashRegisterId!.Value,
                sessionId.Value,
                reception.Id,
                command.UserId,
                command.Request.Amount,
                acceptedAt,
                idempotencyKey,
                command.Request.Description));
        }

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("پذیرش هم‌زمان تغییر کرده است؛ مبلغ مانده را دوباره بررسی کنید.");
    }
}

public sealed class RegisterReceptionPaymentCommandValidator
    : AbstractValidator<RegisterReceptionPaymentCommand>
{
    public RegisterReceptionPaymentCommandValidator()
    {
        RuleFor(command => command.ReceptionId).GreaterThan(0);
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.SalonId).GreaterThan(0);
            RuleFor(command => command.Request.Amount)
                .GreaterThan(0)
                .LessThanOrEqualTo(9999999999999999.99m)
                .Must(value => decimal.Round(value, 2) == value);
            RuleFor(command => command.Request.PaymentMethod).NotEmpty().MaximumLength(50);
            RuleFor(command => command.Request.PaidAt).NotEmpty();
            RuleFor(command => command.Request.TransactionReference).MaximumLength(150);
            RuleFor(command => command.Request.IdempotencyKey).NotEmpty().MaximumLength(100);
            RuleFor(command => command.Request.Description).MaximumLength(1000);
            RuleFor(command => command.Request.CashRegisterId)
                .GreaterThan(0)
                .When(command => string.Equals(
                    command.Request.PaymentMethod?.Trim(),
                    "نقدی",
                    StringComparison.OrdinalIgnoreCase));
            RuleFor(command => command.Request.CashRegisterId)
                .Null()
                .When(command => !string.Equals(
                    command.Request.PaymentMethod?.Trim(),
                    "نقدی",
                    StringComparison.OrdinalIgnoreCase));
        });
    }
}
