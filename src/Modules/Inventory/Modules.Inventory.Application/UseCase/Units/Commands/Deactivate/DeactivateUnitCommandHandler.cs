using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Units;

namespace Modules.Inventory.Application.UseCase;

public sealed class DeactivateUnitCommandHandler(IUnitCommandRepository repository)
    : ICommandHandler<DeactivateUnitCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeactivateUnitCommand command,
        CancellationToken cancellationToken)
    {
        var status = await repository.DeactivateAsync(command.Id, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.NotFound => Result.NotFound("واحد موردنظر پیدا نشد."),
            _ => Result.Error("غیرفعال‌سازی واحد انجام نشد.")
        };
    }
}

public sealed class DeactivateUnitCommandValidator : AbstractValidator<DeactivateUnitCommand>
{
    public DeactivateUnitCommandValidator() =>
        RuleFor(command => command.Id).GreaterThan(0);
}
