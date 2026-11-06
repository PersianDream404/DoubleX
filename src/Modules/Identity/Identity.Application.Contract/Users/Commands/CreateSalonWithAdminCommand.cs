using Framwork.Bus.Command;
using Identity.Application.Contract.DTOs.Users;

namespace Identity.Application.Contract.Users.Commands;

public sealed record CreateSalonWithAdminCommand(
    CreateSalonWithAdminRequestDto Request) : ICommand<CreateSalonWithAdminResponseDto>;
