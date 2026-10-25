using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Materials;

namespace Modules.Inventory.Application.UseCase;

public sealed class CreateMaterialCommandHandler(IMaterialCommandRepository repository)
    : ICommandHandler<CreateMaterialCommand, bool>
{
    public async Task<Result<bool>> Handle(
        CreateMaterialCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var material = Material.Create(
            request.Name,
            request.Code,
            request.Brand,
            request.BaseUnitId,
            request.PurchaseUnitId,
            request.PurchaseConversionFactor,
            request.MinimumStock);
        var status = await repository.AddAsync(material, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.NotFound => Result.NotFound("یکی از واحدهای انتخاب‌شده پیدا نشد."),
            InventoryOperationStatus.Inactive => Result.Error("واحد غیرفعال برای کالای جدید قابل استفاده نیست."),
            InventoryOperationStatus.Conflict => Result.Error("کالایی با این کد از قبل وجود دارد."),
            _ => Result.Error("ثبت کالا انجام نشد.")
        };
    }
}

public sealed class CreateMaterialCommandValidator : AbstractValidator<CreateMaterialCommand>
{
    public CreateMaterialCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
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
