using Framwork.Bus.Query;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

public sealed record GetCashRegisterSessionQuery(long CashRegisterId, long SalonId)
    : IQuery<CashRegisterSessionDto>;
