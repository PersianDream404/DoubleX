using Framwork.Bus.Command;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;

namespace Modules.Service.Application.Contract.UseCase.ServicePersonnels.Commands.Upsert;

public sealed record UpsertServicePersonnelCommand(long? Id, ServicePersonnelRequestDto Request)
    : ICommand<bool>;
