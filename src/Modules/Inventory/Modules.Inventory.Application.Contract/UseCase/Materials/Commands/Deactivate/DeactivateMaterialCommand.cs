using Framwork.Bus.Command;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record DeactivateMaterialCommand(long Id) : ICommand<bool>;
