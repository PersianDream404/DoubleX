using Framwork.Bus.Command;
using Modules.Finance.Application.Contract.DTOs;

namespace Modules.Finance.Application.Contract.UseCase.Commands;

public sealed record RegisterPersonnelSettlementCommand(
    long SalonId,
    long UserId,
    RegisterPersonnelSettlementRequestDto Request) : ICommand<long>;
