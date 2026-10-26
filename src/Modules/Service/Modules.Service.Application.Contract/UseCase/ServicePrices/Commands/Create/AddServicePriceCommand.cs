using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServicePrices;

namespace Modules.Service.Application.Contract.UseCase.ServicePrices.Commands.Create;

public sealed record AddServicePriceCommand(CreateServicePriceRequestDto Request) : ICommand<bool>;
