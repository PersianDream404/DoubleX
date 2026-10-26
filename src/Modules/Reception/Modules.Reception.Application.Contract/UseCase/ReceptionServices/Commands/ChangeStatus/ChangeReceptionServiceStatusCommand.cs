using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.DTOs.ReceptionServices.Add;

namespace Modules.Reception.Application.Contract.UseCase.ReceptionServices.Commands.ChangeStatus;

public sealed record ChangeReceptionServiceStatusCommand(
    long ReceptionId,
    long ServiceId,
    ChangeReceptionServiceStatusRequestDto Request) : ICommand<bool>;
