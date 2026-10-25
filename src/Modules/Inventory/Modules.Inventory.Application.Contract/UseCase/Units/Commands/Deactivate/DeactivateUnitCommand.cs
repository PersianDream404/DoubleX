using Framwork.Bus.Command;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record DeactivateUnitCommand(long Id) : ICommand<bool>;
