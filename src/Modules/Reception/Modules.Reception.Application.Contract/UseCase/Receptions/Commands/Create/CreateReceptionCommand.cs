using Framwork.Bus.Command;
using Modules.Reception.Application.Contract.DTOs.Receptions.Create;

namespace Modules.Reception.Application.Contract.UseCase.Receptions.Commands.Create;

public sealed record CreateReceptionCommand(CreateReceptionRequestDto Request) : ICommand<long>;
