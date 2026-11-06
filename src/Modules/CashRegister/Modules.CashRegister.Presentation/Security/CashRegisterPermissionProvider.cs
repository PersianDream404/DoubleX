using Modules.CashRegister.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.CashRegister.Presentation.Security;

public sealed class CashRegisterPermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        CashRegisterPermissions.List,
        CashRegisterPermissions.Details,
        CashRegisterPermissions.Create,
        CashRegisterPermissions.Update,
        CashRegisterPermissions.Toggle,
        CashRegisterPermissions.SessionsList,
        CashRegisterPermissions.SessionsOpen,
        CashRegisterPermissions.SessionsClose,
        CashRegisterPermissions.SessionsActive,
        CashRegisterPermissions.TransactionsList,
        CashRegisterPermissions.CashIn,
        CashRegisterPermissions.CashOut,
        CashRegisterPermissions.Balance
    ];
}
