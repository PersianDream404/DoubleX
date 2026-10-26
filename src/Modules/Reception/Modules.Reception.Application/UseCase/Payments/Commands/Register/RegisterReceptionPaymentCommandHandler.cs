using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.UseCase.Payments.Commands.Register;

namespace Modules.Reception.Application.UseCase.Payments.Commands.Register;

public sealed class RegisterReceptionPaymentCommandHandler(IReceptionCommandRepository repository)
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

        if (!reception.TryAddPayment(
                command.Request.Amount,
                command.Request.PaymentMethod,
                command.Request.PaidAt,
                command.Request.TransactionReference,
                command.Request.IdempotencyKey,
                command.Request.Description))
        {
            return Result.Error("پرداخت تکراری، نامعتبر یا بیشتر از ماندهٔ همین پذیرش است.");
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
        });
    }
}
