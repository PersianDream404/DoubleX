using Framwork.Bus.Command;

namespace Modules.Service.Application.Contract.UseCase.Services.Commands.Deactivate;

public sealed record DeactivateServiceCommand(long SalonId, long Id) : ICommand<bool>;
