using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.DTOs.ReceptionServiceMaterials.Register;

namespace Modules.Reception.Application.Contract.UseCase.ReceptionServiceMaterials.Commands.Register;

public sealed record RegisterReceptionServiceMaterialCommand(
    long ReceptionId,
    long ReceptionServiceId,
    RegisterReceptionServiceMaterialRequestDto Request) : ICommand<bool>;
