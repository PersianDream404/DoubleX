namespace Modules.CashRegister.Domain.Entities;

public enum CashRegisterTransactionType
{
    OpeningBalance = 1,
    ReceptionPayment = 2,
    CashIn = 3,
    CashOut = 4
}
