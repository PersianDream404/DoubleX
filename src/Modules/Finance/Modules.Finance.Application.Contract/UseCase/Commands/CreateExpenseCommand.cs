using Framwork.Bus.Command;
using Modules.Finance.Application.Contract.DTOs;

namespace Modules.Finance.Application.Contract.UseCase.Commands;

public sealed record CreateExpenseCommand(long SalonId, long UserId, CreateExpenseRequestDto Request)
    : ICommand<long>;
