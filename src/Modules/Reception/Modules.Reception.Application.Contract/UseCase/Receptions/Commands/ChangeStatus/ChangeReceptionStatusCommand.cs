using Framwork.Bus.Command;
using Modules.Reception.Domain.Entities;

namespace Modules.Reception.Application.Contract.UseCase.Receptions.Commands.ChangeStatus;

public sealed record ChangeReceptionStatusCommand(
    long Id,
    long SalonId,
    ReceptionStatus Status) : ICommand<bool>;
