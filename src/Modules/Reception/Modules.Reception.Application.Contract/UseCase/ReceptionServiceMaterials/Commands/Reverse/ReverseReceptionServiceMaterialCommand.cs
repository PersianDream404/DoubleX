using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.DTOs.ReceptionServiceMaterials.Register;

namespace Modules.Reception.Application.Contract.UseCase.ReceptionServiceMaterials.Commands.Reverse;

public sealed record ReverseReceptionServiceMaterialCommand(
    long ReceptionId,
    long ReceptionServiceId,
    long MaterialRecordId,
    ReverseReceptionServiceMaterialRequestDto Request) : ICommand<bool>;
