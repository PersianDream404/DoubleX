using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.DTOs.Materials;

namespace Modules.Inventory.Application.Contract.UseCase;

public sealed record UpdateMaterialCommand(UpdateMaterialRequestDto Request) : ICommand<bool>;
