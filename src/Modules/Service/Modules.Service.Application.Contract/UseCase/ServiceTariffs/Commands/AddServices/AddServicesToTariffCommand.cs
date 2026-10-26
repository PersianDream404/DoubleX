using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;

namespace Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.AddServices;

public sealed record AddServicesToTariffCommand(
    long TariffId,
    AddServiceTariffItemsRequestDto Request) : ICommand<bool>;
