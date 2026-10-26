using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.DTOs.ReceptionServices.Add;

namespace Modules.Reception.Application.Contract.UseCase.ReceptionServices.Commands.Add;

public sealed record AddReceptionServiceCommand(
    long ReceptionId,
    AddReceptionServiceRequestDto Request) : ICommand<bool>;
