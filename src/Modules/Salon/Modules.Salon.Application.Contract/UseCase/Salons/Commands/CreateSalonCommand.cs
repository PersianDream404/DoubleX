using Framwork.Bus.Command;
using Modules.Salon.Application.Contract.DTOs.Salons.Create;
using Modules.Salon.Application.Contract.DTOs.Salons.Update;

namespace Modules.Salon.Application.Contract.UseCase.Salons.Commands;

public record CreateSalonCommand(CreateSalonRequestDto request) : ICommand<bool>;
public record UpdateSalonCommand(UpdateSalonRequestDto request) : ICommand<bool>;
public record DeleteSalonCommand(long Id) : ICommand<bool>;
public record ToggleSalonCommand(long Id) : ICommand<bool>;
