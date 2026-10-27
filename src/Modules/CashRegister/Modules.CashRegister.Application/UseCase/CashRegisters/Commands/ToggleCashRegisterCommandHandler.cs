using Ardalis.Result;
using Framwork.Bus.Command;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Commands;

public sealed class ToggleCashRegisterCommandHandler(ICashRegisterCommandRepository repository)
    : ICommandHandler<ToggleCashRegisterCommand, bool>
{
    public async Task<Result<bool>> Handle(ToggleCashRegisterCommand command, CancellationToken cancellationToken)
    {
        var register = await repository.GetRegisterAsync(command.Id, command.SalonId, cancellationToken);
        if (register is null)
            return Result.NotFound("صندوق موردنظر پیدا نشد.");

        if (register.Status && await repository.GetOpenSessionAsync(command.Id, command.SalonId, cancellationToken) is not null)
            return Result.Conflict("برای غیرفعال‌کردن صندوق ابتدا جلسهٔ باز آن را ببندید.");

        register.SetActive(!register.Status);
        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("وضعیت صندوق هم‌زمان تغییر کرده است.");
    }
}
