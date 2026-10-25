using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.DTOs.Units;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record UpdateUnitCommand(UpdateUnitRequestDto Request) : ICommand<bool>;
