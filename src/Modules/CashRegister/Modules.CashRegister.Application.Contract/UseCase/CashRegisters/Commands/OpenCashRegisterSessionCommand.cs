using Framwork.Bus.Command;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

public sealed record OpenCashRegisterSessionCommand(
    long CashRegisterId,
    long SalonId,
    long UserId,
    decimal OpeningBalance) : ICommand<long>;
