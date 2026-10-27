using Framwork.Bus.Command;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

public sealed record CloseCashRegisterSessionCommand(
    long CashRegisterId,
    long SessionId,
    long SalonId,
    long UserId,
    decimal ActualClosingBalance) : ICommand<bool>;
