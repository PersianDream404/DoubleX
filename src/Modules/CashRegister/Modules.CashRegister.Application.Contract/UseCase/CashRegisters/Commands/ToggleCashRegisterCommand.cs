using Framwork.Bus.Command;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

public sealed record ToggleCashRegisterCommand(long Id, long SalonId) : ICommand<bool>;
