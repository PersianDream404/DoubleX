using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Finance.Application.Contract.DTOs;
using Modules.Finance.Application.Contract.UseCase.Commands;
using Modules.Finance.Domain.Entities;
using Modules.Finance.Domain.Interfaces;

namespace Modules.Finance.Application.UseCase.Commands;

public sealed class CreateExpenseCommandHandler(IFinanceCommandRepository repository)
    : ICommandHandler<CreateExpenseCommand, long>
{
    public async Task<Result<long>> Handle(
        CreateExpenseCommand command,
        CancellationToken cancellationToken)
    {
        var expense = Expense.Create(
            command.SalonId,
            command.UserId,
            command.Request.Category,
            command.Request.Amount,
            command.Request.OccurredAt,
            command.Request.Description,
            command.Request.IdempotencyKey);
        if (!await repository.AddExpenseAsync(expense, cancellationToken))
        {
            return Result.Conflict("هزینه با این کلید قبلاً ثبت شده یا ذخیرهٔ آن ناموفق بوده است.");
        }

        return Result.Success(expense.Id);
    }
}

public sealed class CreateExpenseCommandValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.UserId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        RuleFor(command => command.Request.Category).NotEmpty().MaximumLength(100);
        RuleFor(command => command.Request.Amount)
            .GreaterThan(0)
            .Must(amount => decimal.Round(amount, 2) == amount);
        RuleFor(command => command.Request.OccurredAt).NotEmpty();
        RuleFor(command => command.Request.Description).MaximumLength(1000);
        RuleFor(command => command.Request.IdempotencyKey).NotEmpty().MaximumLength(100);
    }
}
