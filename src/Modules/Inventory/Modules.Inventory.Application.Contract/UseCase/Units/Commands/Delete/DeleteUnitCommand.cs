using Framwork.Bus.Command;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record DeleteUnitCommand(long Id) : ICommand<bool>;
