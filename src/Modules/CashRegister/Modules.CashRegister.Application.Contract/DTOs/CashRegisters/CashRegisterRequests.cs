namespace Modules.CashRegister.Application.Contract.DTOs.CashRegisters;

public sealed record CreateCashRegisterRequestDto(
    string Name,
    string Code,
    string? Description);

public sealed record UpdateCashRegisterRequestDto(
    string Name,
    string Code,
    string? Description);
