using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.DTOs.Units;
using Modules.Inventory.Application.Contract.Interfaces.Units;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Units;

namespace Modules.Inventory.Application.UseCase;

public sealed class UpdateUnitCommandHandler(
    IUnitCommandRepository commandRepository,
    IUnitQueryRepository queryRepository)
    : ICommandHandler<UpdateUnitCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateUnitCommand command,
        CancellationToken cancellationToken)
    {
        var request = command.Request;
        var unit = await queryRepository.GetEntityByIdAsync(request.Id, cancellationToken);
        if (unit is null)
        {
            return Result.NotFound("واحد موردنظر پیدا نشد.");
        }

        unit.Update(request.Name, request.Symbol, request.UnitType, request.IsActive);
        var status = await commandRepository.UpdateAsync(unit, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.Conflict => Result.Error("واحدی با این نام یا نماد از قبل وجود دارد."),
            InventoryOperationStatus.NotFound => Result.NotFound("واحد موردنظر پیدا نشد."),
            _ => Result.Error("ویرایش واحد انجام نشد.")
        };
    }
}

public sealed class UpdateUnitCommandValidator : AbstractValidator<UpdateUnitCommand>
{
    public UpdateUnitCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.Id).GreaterThan(0);
            RuleFor(command => command.Request.Name).NotEmpty().MaximumLength(100);
            RuleFor(command => command.Request.Symbol).NotEmpty().MaximumLength(20);
            RuleFor(command => command.Request.UnitType).IsInEnum();
        });
    }
}
