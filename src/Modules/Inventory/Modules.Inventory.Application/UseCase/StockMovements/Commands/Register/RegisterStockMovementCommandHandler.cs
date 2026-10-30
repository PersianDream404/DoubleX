using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.StockMovements;

namespace Modules.Inventory.Application.UseCase;

public sealed class RegisterStockMovementCommandHandler(IStockMovementCommandRepository repository)
    : ICommandHandler<RegisterStockMovementCommand, RegisterStockMovementResponseDto>
{
    public async Task<Result<RegisterStockMovementResponseDto>> Handle(
        RegisterStockMovementCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var movement = StockMovement.Create(
            request.MaterialId,
            request.Type,
            request.Quantity,
            request.PurchaseQuantity,
            request.PurchaseUnitPrice,
            request.OccurredAt,
            request.ReferenceType?.Trim(),
            request.ReferenceId,
            request.Description?.Trim(),
            request.IdempotencyKey?.Trim());
        var status = await repository.RegisterAsync(
            movement,
            command.MaximumTotalCost,
            cancellationToken);
        return status.Status switch
        {
            InventoryOperationStatus.Succeeded =>
                Result.Success(new RegisterStockMovementResponseDto(
                    status.UnitCost,
                    status.TotalCost)),
            InventoryOperationStatus.NotFound => Result.NotFound("کالای موردنظر پیدا نشد."),
            InventoryOperationStatus.Inactive => Result.Error("برای کالای غیرفعال گردش ثبت نمی‌شود."),
            InventoryOperationStatus.Conflict => Result.Error("این کلید جلوگیری از ثبت تکراری قبلاً استفاده شده است."),
            InventoryOperationStatus.HistoricalDataConflict =>
                Result.Error("بهای FIFO این موجودی مشخص نیست؛ برای ثبت مصرف، ابتدا موجودیِ بدون قیمت خرید را تعیین تکلیف کنید."),
            InventoryOperationStatus.AmountLimitExceeded =>
                Result.Error("هزینهٔ FIFO این ماده از سقف مبلغ مجاز پذیرش بیشتر است."),
            InventoryOperationStatus.InvalidInput => Result.Error("اطلاعات مقدار یا قیمت گردش انبار معتبر نیست."),
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
        RuleFor(command => command.MaximumTotalCost)
            .GreaterThanOrEqualTo(0)
            .When(command => command.MaximumTotalCost.HasValue);
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.MaterialId).GreaterThan(0);
            RuleFor(command => command.Request.Type).IsInEnum();
            RuleFor(command => command.Request.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(999999999999999.999m)
                .Must(value => decimal.Round(value, 3) == value);
            When(command => command.Request.Type == StockMovementType.Receipt, () =>
            {
                RuleFor(command => command.Request.PurchaseQuantity)
                    .NotNull()
                    .GreaterThan(0)
                    .LessThanOrEqualTo(999999999999.999999m)
                    .Must(value => value.HasValue && decimal.Round(value.Value, 6) == value.Value);
                RuleFor(command => command.Request.PurchaseUnitPrice)
                    .NotNull()
                    .GreaterThanOrEqualTo(0)
                    .LessThanOrEqualTo(9999999999999999.99m)
                    .Must(value => value.HasValue && decimal.Round(value.Value, 2) == value.Value);
            });
            When(command => command.Request.Type != StockMovementType.Receipt, () =>
            {
                RuleFor(command => command.Request.PurchaseQuantity).Null();
                RuleFor(command => command.Request.PurchaseUnitPrice).Null();
            });
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
            RuleFor(command => command.Request.ReferenceType)
                .Equal("ReceptionServiceMaterial")
                .When(command => command.Request.Type == StockMovementType.Return);
            RuleFor(command => command.Request.ReferenceId)
                .NotNull()
                .GreaterThan(0)
                .When(command => command.Request.Type == StockMovementType.Return);
            RuleFor(command => command.Request.IdempotencyKey)
                .NotEmpty()
                .When(command => command.Request.Type == StockMovementType.Return);
        });
    }
}
