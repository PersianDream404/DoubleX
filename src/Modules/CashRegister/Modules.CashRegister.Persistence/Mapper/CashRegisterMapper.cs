using System.Linq.Expressions;
using Modules.CashRegister.Application.Contract.DTOs.CashRegisters;
using Modules.CashRegister.Application.Contract.DTOs.Sessions;
using Modules.CashRegister.Application.Contract.DTOs.Transactions;
using Modules.CashRegister.Domain.Entities;
using CashRegisterEntity = Modules.CashRegister.Domain.Entities.CashRegister;

namespace Modules.CashRegister.Persistence.Mapper;

public static class CashRegisterMapper
{
    public static Expression<Func<CashRegisterEntity, CashRegisterDto>> ToCashRegisterDto() =>
        register => new CashRegisterDto(
            register.Id,
            register.SalonId,
            register.Name,
            register.Code,
            register.Description,
            register.Status,
            register.Sessions
                .Where(session => session.SessionStatus == CashRegisterSessionStatus.Open)
                .Select(session => (long?)session.Id)
                .FirstOrDefault());

    public static Expression<Func<CashRegisterSession, CashRegisterSessionDto>> ToSessionDto() =>
        session => new CashRegisterSessionDto(
            session.Id,
            session.SalonId,
            session.CashRegisterId,
            session.OpenedByUserId,
            session.OpenedAt,
            session.OpeningBalance,
            session.ClosedByUserId,
            session.ClosedAt,
            session.ExpectedClosingBalance,
            session.ActualClosingBalance,
            session.Difference,
            session.SessionStatus);

    public static Expression<Func<CashRegisterTransaction, CashRegisterTransactionDto>> ToTransactionDto() =>
        transaction => new CashRegisterTransactionDto(
            transaction.Id,
            transaction.CashRegisterId,
            transaction.SessionId,
            transaction.Type,
            transaction.Amount,
            transaction.OccurredAt,
            transaction.CreatedByUserId,
            transaction.Description,
            transaction.ReferenceType,
            transaction.ReferenceId);
}
