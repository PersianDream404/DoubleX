using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;
using Framwork.Bus.Command;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

public sealed record CreateCashRegisterCommand(
    long SalonId,
    CreateCashRegisterRequestDto Request) : ICommand<long>;
