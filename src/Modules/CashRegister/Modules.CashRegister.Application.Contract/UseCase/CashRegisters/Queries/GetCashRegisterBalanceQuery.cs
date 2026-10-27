using Framwork.Bus.Query;
using Modules.CashRegister.Application.Contract.DTOs.Transactions;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

public sealed record GetCashRegisterBalanceQuery(long CashRegisterId, long SalonId)
    : IQuery<CashRegisterBalanceDto>;
