using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Units;

namespace Modules.Inventory.Application.UseCase;

public sealed class DeleteUnitCommandHandler(IUnitCommandRepository repository)
    : ICommandHandler<DeleteUnitCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteUnitCommand command,
        CancellationToken cancellationToken)
    {
        var status = await repository.DeleteAsync(command.Id, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.NotFound => Result.NotFound("واحد موردنظر پیدا نشد."),
            InventoryOperationStatus.Conflict => Result.Error("واحدی که در کالاها استفاده شده قابل حذف نیست."),
            _ => Result.Error("حذف واحد انجام نشد.")
        };
    }
}

public sealed class DeleteUnitCommandValidator : AbstractValidator<DeleteUnitCommand>
{
    public DeleteUnitCommandValidator() =>
        RuleFor(command => command.Id).GreaterThan(0);
}
