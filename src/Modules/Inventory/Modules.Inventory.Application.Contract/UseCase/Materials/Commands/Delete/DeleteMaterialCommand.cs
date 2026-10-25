using Framwork.Bus.Command;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record DeleteMaterialCommand(long Id) : ICommand<bool>;
