using Framwork.Bus.Command;
using Modules.Personnel.Application.Contract.DTOs.Personnel;

namespace Modules.Personnel.Application.Contract.UseCase.Personnel.Commands;

public record CreatePersonnelCommand(CreatePersonnelRequestDto Request) : ICommand<long>;
public record UpdatePersonnelCommand(long Id, UpdatePersonnelRequestDto Request) : ICommand<bool>;
public record TogglePersonnelCommand(long Id) : ICommand<bool>;
public record DeletePersonnelCommand(long Id) : ICommand<bool>;
