using Framwork.Bus.Command;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

public sealed record RegisterReceptionPaymentTransactionCommand(
    long SalonId,
    long CashRegisterId,
    long SessionId,
    long ReceptionId,
    long UserId,
    decimal Amount,
    DateTimeOffset OccurredAt,
    string IdempotencyKey,
    string? Description) : ICommand<bool>;
