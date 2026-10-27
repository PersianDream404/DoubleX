using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

public sealed record GetCashRegistersQuery(CashRegisterListRequestDto Request)
    : IQuery<PagedList<CashRegisterDto>>;
