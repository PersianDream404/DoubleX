using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

public sealed record GetCashRegisterSessionsQuery(
    long CashRegisterId,
    long SalonId,
    int PageNumber,
    int PageSize) : IQuery<PagedList<CashRegisterSessionDto>>;
