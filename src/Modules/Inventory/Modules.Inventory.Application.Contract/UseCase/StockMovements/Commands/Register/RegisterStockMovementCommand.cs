using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.DTOs;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record RegisterStockMovementCommand(RegisterStockMovementRequestDto Request) : ICommand<bool>;
