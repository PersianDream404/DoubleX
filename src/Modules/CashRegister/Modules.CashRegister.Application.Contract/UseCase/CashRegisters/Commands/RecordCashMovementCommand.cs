using Framwork.Bus.Command;
using Modules.CashRegister.Domain.Entities;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

public sealed record RecordCashMovementCommand(
    long CashRegisterId,
    long SalonId,
    long UserId,
    CashRegisterTransactionType Type,
    decimal Amount,
    string IdempotencyKey,
    string? Description) : ICommand<bool>;
