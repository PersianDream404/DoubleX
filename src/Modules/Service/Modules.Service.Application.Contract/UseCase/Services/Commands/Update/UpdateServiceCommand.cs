using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.Services;

namespace Modules.Service.Application.Contract.UseCase.Services.Commands.Update;

public sealed record UpdateServiceCommand(long Id, UpdateServiceRequestDto Request) : ICommand<bool>;
