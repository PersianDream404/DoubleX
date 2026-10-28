using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Finance.Application.Contract.UseCase.Commands;
using Modules.Finance.Domain.Interfaces;

namespace Modules.Finance.Application.UseCase.Commands;

public sealed class CancelExpenseCommandHandler(IFinanceCommandRepository repository)
    : ICommandHandler<CancelExpenseCommand, bool>
{
    public async Task<Result<bool>> Handle(
        CancelExpenseCommand command,
        CancellationToken cancellationToken)
    {
        var expense = await repository.GetExpenseAsync(command.Id, command.SalonId, cancellationToken);
        if (expense is null)
        {
            return Result.NotFound("هزینهٔ موردنظر پیدا نشد.");
        }

        if (!expense.Cancel(DateTimeOffset.UtcNow, command.Reason))
        {
            return Result.Conflict("هزینه قبلاً لغو شده یا دلیل لغو معتبر نیست.");
        }

        return await repository.SaveChangesAsync(cancellationToken)
            ? Result.Success(true)
            : Result.Conflict("لغو هزینه ذخیره نشد.");
    }
}

public sealed class CancelExpenseCommandValidator : AbstractValidator<CancelExpenseCommand>
{
    public CancelExpenseCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Reason).NotEmpty().MaximumLength(500);
    }
}
