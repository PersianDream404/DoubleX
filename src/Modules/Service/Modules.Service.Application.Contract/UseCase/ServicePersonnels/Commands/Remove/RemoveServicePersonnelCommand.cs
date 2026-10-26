using Framwork.Bus.Command;

namespace Modules.Service.Application.Contract.UseCase.ServicePersonnels.Commands.Remove;

public sealed record RemoveServicePersonnelCommand(long SalonId, long ServiceId, long Id)
    : ICommand<bool>;
