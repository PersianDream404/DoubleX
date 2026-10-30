using System.Security.Cryptography;
using System.Text;
using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Extensions;
using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.UseCase.ReceptionServiceMaterials.Commands.Reverse;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.UseCase.ReceptionServiceMaterials.Commands.Reverse;

public sealed class ReverseReceptionServiceMaterialCommandHandler(
    IReceptionCommandRepository repository,
    ICommandBus commandBus)
    : ICommandHandler<ReverseReceptionServiceMaterialCommand, bool>
{
    public async Task<Result<bool>> Handle(
        ReverseReceptionServiceMaterialCommand command,
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

        var service = reception.Services.SingleOrDefault(item => item.Id == command.ReceptionServiceId);
        var material = service?.Materials.SingleOrDefault(item => item.Id == command.MaterialRecordId);
        if (service is null || material is null)
        {
            return Result.NotFound("رکورد مصرف موردنظر پیدا نشد.");
        }

        if (!material.MovementRegistered)
        {
            return Result.Error("مصرف ثبت‌شده‌ای برای معکوس‌سازی وجود ندارد.");
        }
        if (material.MovementReversed)
        {
            return Result.Success(true);
        }
        if (!reception.CanReverseMaterialMovement(command.ReceptionServiceId, material.Id))
        {
            return Result.Error("به‌دلیل پرداخت ثبت‌شده، برگشت این ماده مبلغ پذیرش را کمتر از مبلغ پرداختی می‌کند.");
        }

        var returnKey = Convert.ToHexString(SHA256.HashData(
            Encoding.UTF8.GetBytes($"reverse:{material.IdempotencyKey}")));
        var result = await commandBus.Send<
            RegisterStockMovementCommand,
            RegisterStockMovementResponseDto>(
            new RegisterStockMovementCommand(new RegisterStockMovementRequestDto
            {
                MaterialId = material.MaterialId,
                Type = StockMovementType.Return,
                Quantity = material.Quantity,
                OccurredAt = DateTimeOffset.UtcNow,
                ReferenceType = "ReceptionServiceMaterial",
                ReferenceId = material.Id,
                Description = "معکوس‌سازی مصرف ثبت‌شده در پذیرش",
                IdempotencyKey = returnKey
            }),
            cancellationToken);
        if (!result.IsSuccess)
        {
            return Result.Error(result.GetErrorMessage());
        }

        if (!reception.MarkMaterialMovementReversed(command.ReceptionServiceId, material.Id))
        {
            return Result.Error("گردش برگشت ثبت شد، اما وضعیت رکورد مصرف در پذیرش به‌روزرسانی نشد.");
        }

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("گردش برگشت ثبت شد؛ وضعیت پذیرش هم‌زمان تغییر کرد و باید دوباره بررسی شود.");
    }
}

public sealed class ReverseReceptionServiceMaterialCommandValidator
    : AbstractValidator<ReverseReceptionServiceMaterialCommand>
{
    public ReverseReceptionServiceMaterialCommandValidator()
    {
        RuleFor(command => command.ReceptionId).GreaterThan(0);
        RuleFor(command => command.ReceptionServiceId).GreaterThan(0);
        RuleFor(command => command.MaterialRecordId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
            RuleFor(command => command.Request.SalonId).GreaterThan(0));
    }
}
