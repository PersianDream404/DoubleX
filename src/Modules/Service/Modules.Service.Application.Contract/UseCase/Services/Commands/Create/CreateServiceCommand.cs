using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.Services;

namespace Modules.Service.Application.Contract.UseCase.Services.Commands.Create;

public sealed record CreateServiceCommand(ServiceRequestDto Request) : ICommand<bool>;
