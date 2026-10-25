using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.DTOs.Materials;
using Modules.Inventory.Application.Contract.Interfaces.Materials;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Materials;

namespace Modules.Inventory.Application.UseCase;

public sealed class UpdateMaterialCommandHandler(
    IMaterialCommandRepository commandRepository,
    IMaterialQueryRepository queryRepository)
    : ICommandHandler<UpdateMaterialCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateMaterialCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var material = await queryRepository.GetEntityByIdAsync(request.Id, cancellationToken);
        if (material is null)
        {
            return Result.NotFound("کالای موردنظر پیدا نشد.");
        }

        material.Update(
            request.Name,
            request.Code,
            request.Brand,
            request.BaseUnitId,
            request.PurchaseUnitId,
            request.PurchaseConversionFactor,
            request.MinimumStock,
            request.IsActive);
        var status = await commandRepository.UpdateAsync(material, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.NotFound => Result.NotFound("کالا یا واحد انتخاب‌شده پیدا نشد."),
            InventoryOperationStatus.Inactive => Result.Error("واحد غیرفعال برای کالا قابل استفاده نیست."),
            InventoryOperationStatus.Conflict => Result.Error("کالایی با این کد از قبل وجود دارد."),
            InventoryOperationStatus.HistoricalDataConflict =>
                Result.Error("واحد پایه کالایی که موجودی یا سابقه گردش دارد قابل تغییر نیست."),
            _ => Result.Error("ویرایش کالا انجام نشد.")
        };
    }
}

public sealed class UpdateMaterialCommandValidator : AbstractValidator<UpdateMaterialCommand>
{
    public UpdateMaterialCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.Id).GreaterThan(0);
            RuleFor(command => command.Request.Name).NotEmpty().MaximumLength(200);
            RuleFor(command => command.Request.Code).NotEmpty().MaximumLength(50);
            RuleFor(command => command.Request.Brand).MaximumLength(100);
            RuleFor(command => command.Request.BaseUnitId).GreaterThan(0);
            RuleFor(command => command.Request.PurchaseUnitId).GreaterThan(0);
            RuleFor(command => command.Request.PurchaseConversionFactor)
                .GreaterThan(0)
                .LessThanOrEqualTo(999999999999.999999m)
                .Must(value => decimal.Round(value, 6) == value);
            RuleFor(command => command.Request.MinimumStock)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(999999999999999.999m)
                .Must(value => decimal.Round(value, 3) == value);
        });
    }
}
