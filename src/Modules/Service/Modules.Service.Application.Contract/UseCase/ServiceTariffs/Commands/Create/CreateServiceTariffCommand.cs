using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;

namespace Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.Create;

public sealed record CreateServiceTariffCommand(ServiceTariffRequestDto Request) : ICommand<bool>;
