using Framwork.Bus.Command;

namespace Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.Delete;

public sealed record DeleteServiceTariffCommand(long Id, long SalonId) : ICommand<bool>;
