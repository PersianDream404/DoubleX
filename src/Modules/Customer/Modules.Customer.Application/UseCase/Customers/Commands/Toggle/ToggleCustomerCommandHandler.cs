using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Application.Contract.UseCase.Customers.Commands.Toggle;
using Modules.Customer.Domain.Interface;

namespace Modules.Customer.Application.UseCase.Customers.Commands.Toggle;

public sealed class ToggleCustomerCommandHandler(
    ICustomerCommandRepository commandRepository,
    ICustomerQueryRepository queryRepository)
    : ICommandHandler<ToggleCustomerCommand, bool>
{
    public async Task<Result<bool>> Handle(
        ToggleCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var customer = await queryRepository.GetEntityByIdAsync(command.Id, cancellationToken);
        if (customer is null)
        {
            return Result.NotFound("مشتری موردنظر پیدا نشد.");
        }

        await commandRepository.ToggleAsync(customer, cancellationToken);
        return true;
    }
}

public sealed class ToggleCustomerCommandValidator : AbstractValidator<ToggleCustomerCommand>
{
    public ToggleCustomerCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(SharedValidationMessages.GreaterThanZero);
    }
}
