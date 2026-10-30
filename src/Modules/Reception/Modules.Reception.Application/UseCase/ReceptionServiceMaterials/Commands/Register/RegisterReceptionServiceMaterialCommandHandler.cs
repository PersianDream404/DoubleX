using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using System.Security.Cryptography;
using System.Text;
using Framwork.Extensions;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.DTOs;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Reception.Application.Contract.Interfaces;
using Modules.Reception.Application.Contract.UseCase.ReceptionServiceMaterials.Commands.Register;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.UseCase.ReceptionServiceMaterials.Commands.Register;

public sealed class RegisterReceptionServiceMaterialCommandHandler(
    IReceptionCommandRepository repository,
    IQueryBus queryBus,
    ICommandBus commandBus)
    : ICommandHandler<RegisterReceptionServiceMaterialCommand, bool>
{
    public async Task<Result<bool>> Handle(
        RegisterReceptionServiceMaterialCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var reception = await repository.GetTrackedAsync(
            command.ReceptionId,
            request.SalonId,
            cancellationToken);
        if (reception is null)
        {
            return Result.NotFound("پذیرش موردنظر پیدا نشد.");
        }

        var service = reception.Services.SingleOrDefault(item => item.Id == command.ReceptionServiceId);
        if (service is null)
        {
            return Result.NotFound("خدمت موردنظر در این پذیرش پیدا نشد.");
        }
        if ((reception.ReceptionStatus is ReceptionStatus.Completed or ReceptionStatus.Cancelled) &&
            !service.Materials.Any(item =>
                item.IdempotencyKey == CreateInventoryIdempotencyKey(
                    command.ReceptionId,
                    command.ReceptionServiceId,
                    request.IdempotencyKey)))
        {
            return Result.Error("ثبت مصرف جدید برای پذیرش نهایی‌شده یا لغوشده مجاز نیست.");
        }

        var materialResult = await queryBus.Send<GetMaterialByIdQuery, MaterialDto>(
            new GetMaterialByIdQuery(request.MaterialId),
            cancellationToken);
        if (!materialResult.IsSuccess || !materialResult.Value.IsActive)
        {
            return Result.Invalid(new ValidationError("مادهٔ فعال موردنظر پیدا نشد."));
        }

        var idempotencyKey = CreateInventoryIdempotencyKey(
            command.ReceptionId,
            command.ReceptionServiceId,
            request.IdempotencyKey);
        var existing = service.Materials.SingleOrDefault(item => item.IdempotencyKey == idempotencyKey);
        if (existing is not null &&
            (existing.MaterialId != request.MaterialId || existing.Quantity != request.Quantity))
        {
            return Result.Conflict("کلید جلوگیری از ثبت تکراری با اطلاعات دیگری استفاده شده است.");
        }

        if (existing is null)
        {
            if (!service.TryAddMaterial(
                    request.MaterialId,
                    request.Quantity,
                    idempotencyKey,
                    request.Description))
            {
                return Result.Error("ثبت مصرف برای این خدمت در وضعیت فعلی مجاز نیست.");
            }

            if (!await repository.SaveChangesAsync(cancellationToken))
            {
                return Result.Conflict("پذیرش هم‌زمان تغییر کرده است.");
            }
        }
        else if (existing.MovementRegistered && existing.UnitCost is not null)
        {
            return Result.Success(true);
        }
        else if (existing.MovementRegistered)
        {
            return Result.Error("برای مصرف ثبت‌شدهٔ قدیمی، بهای FIFO قابل بازیابی نیست.");
        }

        var receptionMaterial = service.Materials.SingleOrDefault(item =>
            item.IdempotencyKey == idempotencyKey);
        if (receptionMaterial is null)
        {
            return Result.Error("رکورد مادهٔ مصرفی در پذیرش پیدا نشد.");
        }

        var movementResult = await commandBus.Send<
            RegisterStockMovementCommand,
            RegisterStockMovementResponseDto>(
            new RegisterStockMovementCommand(new RegisterStockMovementRequestDto
            {
                MaterialId = request.MaterialId,
                Type = StockMovementType.Consumption,
                Quantity = request.Quantity,
                OccurredAt = DateTimeOffset.UtcNow,
                ReferenceType = "ReceptionServiceMaterial",
                ReferenceId = receptionMaterial.Id,
                Description = request.Description,
                IdempotencyKey = idempotencyKey
            }, reception.MaximumAdditionalMaterialCharge),
            cancellationToken);
        if (!movementResult.IsSuccess)
        {
            return Result.Error(movementResult.GetErrorMessage());
        }

        if (movementResult.Value.UnitCost is null)
        {
            return Result.Error("گردش انبار ثبت شد، اما بهای FIFO برای مادهٔ مصرفی در دسترس نیست.");
        }
        if (movementResult.Value.TotalCost is null)
        {
            return Result.Error("گردش انبار ثبت شد، اما مبلغ FIFO برای مادهٔ مصرفی در دسترس نیست.");
        }

        if (!reception.MarkMaterialMovementRegistered(
                command.ReceptionServiceId,
                idempotencyKey,
                movementResult.Value.UnitCost.Value,
                movementResult.Value.TotalCost.Value))
        {
            return Result.Error("گردش انبار ثبت شد، اما مبلغ ماده از محدودهٔ مجاز پذیرش بیشتر است.");
        }

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("گردش انبار ثبت شد؛ وضعیت پذیرش هم‌زمان تغییر کرد و باید دوباره بررسی شود.");
    }

    private static string CreateInventoryIdempotencyKey(
        long receptionId,
        long receptionServiceId,
        string clientKey)
    {
        var value = $"reception:{receptionId}:service:{receptionServiceId}:{clientKey.Trim()}";
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
    }
}

public sealed class RegisterReceptionServiceMaterialCommandValidator
    : AbstractValidator<RegisterReceptionServiceMaterialCommand>
{
    public RegisterReceptionServiceMaterialCommandValidator()
    {
        RuleFor(command => command.ReceptionId).GreaterThan(0);
        RuleFor(command => command.ReceptionServiceId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.SalonId).GreaterThan(0);
            RuleFor(command => command.Request.MaterialId).GreaterThan(0);
            RuleFor(command => command.Request.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(999999999999999.999m)
                .Must(value => decimal.Round(value, 3) == value);
            RuleFor(command => command.Request.IdempotencyKey).NotEmpty().MaximumLength(100);
            RuleFor(command => command.Request.Description).MaximumLength(1000);
        });
    }
}
