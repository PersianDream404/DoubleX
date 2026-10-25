using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Materials;

namespace Modules.Inventory.Application.UseCase;

public sealed class DeleteMaterialCommandHandler(IMaterialCommandRepository repository)
    : ICommandHandler<DeleteMaterialCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteMaterialCommand command,
        CancellationToken cancellationToken)
    {
        var status = await repository.DeleteAsync(command.Id, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.NotFound => Result.NotFound("کالای موردنظر پیدا نشد."),
            _ => Result.Error("حذف کالا انجام نشد.")
        };
    }
}

public sealed class DeleteMaterialCommandValidator : AbstractValidator<DeleteMaterialCommand>
{
    public DeleteMaterialCommandValidator() =>
        RuleFor(command => command.Id).GreaterThan(0);
}
