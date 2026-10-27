using Framwork.Bus.Query;
using Framwork.PagedList;
using Modules.CashRegister.Application.Contract.DTOs.Transactions;

namespace Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Queries;

public sealed record GetCashRegisterTransactionsQuery(
    long CashRegisterId,
    CashRegisterTransactionListRequestDto Request) : IQuery<PagedList<CashRegisterTransactionDto>>;
