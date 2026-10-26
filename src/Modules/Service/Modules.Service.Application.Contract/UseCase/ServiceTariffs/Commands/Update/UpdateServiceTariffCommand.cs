using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;

namespace Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.Update;

public sealed record UpdateServiceTariffCommand(long Id, ServiceTariffRequestDto Request) : ICommand<bool>;
