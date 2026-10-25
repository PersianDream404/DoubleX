using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.DTOs;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record CreateUnitCommand(CreateUnitRequestDto Request) : ICommand<bool>;
