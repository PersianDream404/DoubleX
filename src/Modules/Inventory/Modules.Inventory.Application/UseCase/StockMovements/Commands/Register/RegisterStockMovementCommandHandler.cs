using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.StockMovements;

namespace Modules.Inventory.Application.UseCase;

public sealed class RegisterStockMovementCommandHandler(IStockMovementCommandRepository repository)
    : ICommandHandler<RegisterStockMovementCommand, bool>
{
    public async Task<Result<bool>> Handle(
        RegisterStockMovementCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var movement = StockMovement.Create(
            request.MaterialId,
            request.Type,
            request.Quantity,
            request.OccurredAt,
            request.ReferenceType?.Trim(),
            request.ReferenceId,
            request.Description?.Trim(),
            request.IdempotencyKey?.Trim());
        var status = await repository.RegisterAsync(movement, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.NotFound => Result.NotFound("کالای موردنظر پیدا نشد."),
            InventoryOperationStatus.Inactive => Result.Error("برای کالای غیرفعال گردش ثبت نمی‌شود."),
            InventoryOperationStatus.Conflict => Result.Error("این کلید جلوگیری از ثبت تکراری قبلاً استفاده شده است."),
            InventoryOperationStatus.InvalidInput => Result.Error("برای مصرف کالا، کلید یکتای جلوگیری از ثبت تکراری الزامی است."),
            InventoryOperationStatus.InsufficientStock => Result.Error("موجودی برای این خروج کافی نیست."),
            InventoryOperationStatus.ConcurrencyConflict => Result.Error("تغییر هم‌زمان موجودی رخ داده است؛ عملیات را دوباره بررسی کنید."),
            _ => Result.Error("ثبت گردش انبار انجام نشد.")
        };
    }
}

public sealed class RegisterStockMovementCommandValidator : AbstractValidator<RegisterStockMovementCommand>
{
    public RegisterStockMovementCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.MaterialId).GreaterThan(0);
            RuleFor(command => command.Request.Type).IsInEnum();
            RuleFor(command => command.Request.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(999999999999999.999m)
                .Must(value => decimal.Round(value, 3) == value);
            RuleFor(command => command.Request.OccurredAt).NotEmpty();
            RuleFor(command => command.Request.ReferenceType).MaximumLength(50);
            RuleFor(command => command.Request.ReferenceId).GreaterThan(0).When(command =>
                command.Request.ReferenceId.HasValue);
            RuleFor(command => command.Request.Description).MaximumLength(1000);
            RuleFor(command => command.Request.IdempotencyKey).MaximumLength(100).When(command =>
                command.Request.IdempotencyKey is not null);
            RuleFor(command => command.Request.IdempotencyKey)
                .NotEmpty()
                .When(command => command.Request.Type == StockMovementType.Consumption);
        });
    }
}
