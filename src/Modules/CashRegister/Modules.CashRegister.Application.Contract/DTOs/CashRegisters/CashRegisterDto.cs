namespace Modules.CashRegister.Application.Contract.DTOs.CashRegisters;

public sealed record CashRegisterDto(
    long Id,
    long SalonId,
    string Name,
    string Code,
    string? Description,
    bool IsActive,
    long? ActiveSessionId);
