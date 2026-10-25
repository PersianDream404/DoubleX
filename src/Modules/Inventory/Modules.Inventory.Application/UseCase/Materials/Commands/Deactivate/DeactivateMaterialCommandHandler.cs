using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Materials;

namespace Modules.Inventory.Application.UseCase;

public sealed class DeactivateMaterialCommandHandler(IMaterialCommandRepository repository)
    : ICommandHandler<DeactivateMaterialCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeactivateMaterialCommand command,
        CancellationToken cancellationToken)
    {
        var status = await repository.DeactivateAsync(command.Id, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.NotFound => Result.NotFound("کالای موردنظر پیدا نشد."),
            _ => Result.Error("غیرفعال‌سازی کالا انجام نشد.")
        };
    }
}

public sealed class DeactivateMaterialCommandValidator : AbstractValidator<DeactivateMaterialCommand>
{
    public DeactivateMaterialCommandValidator() =>
        RuleFor(command => command.Id).GreaterThan(0);
}
