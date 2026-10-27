using Framwork.Bus.Query;
using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

public sealed record GetCashRegisterByIdQuery(long Id, long SalonId) : IQuery<CashRegisterDto>;
