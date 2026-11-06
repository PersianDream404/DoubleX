using Modules.Finance.Application.Contract.Security;
using SharedKernel.Interface;

namespace Modules.Finance.Presentation.Security;

public sealed class FinancePermissionProvider : IModulePermissionProvider
{
    public IEnumerable<string> GetPermissions() =>
    [
        FinancePermissions.ExpensesList,
        FinancePermissions.ExpensesDetails,
        FinancePermissions.ExpensesCreate,
        FinancePermissions.ExpensesCancel,
        FinancePermissions.Reports,
        FinancePermissions.EarningsCalculate,
        FinancePermissions.SettlementsList,
        FinancePermissions.SettlementsRegister
    ];
}
