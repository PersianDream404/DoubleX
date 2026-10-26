using Framwork.Bus.Command;

namespace Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.RemoveService;

public sealed record RemoveServiceFromTariffCommand(long TariffId, long ServiceId, long SalonId) : ICommand<bool>;
