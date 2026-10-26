using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;

namespace Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.UpdateServiceAmount;

public sealed record UpdateServiceTariffItemCommand(
    long TariffId,
    long ServiceId,
    UpdateServiceTariffItemRequestDto Request) : ICommand<bool>;
