using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Validation.Resources;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Application.Contract.UseCase.Customers.Commands.Delete;
using Modules.Customer.Domain.Interface;

namespace Modules.Customer.Application.UseCase.Customers.Commands.Delete;

public sealed class DeleteCustomerCommandHandler(
    ICustomerCommandRepository commandRepository,
    ICustomerQueryRepository queryRepository)
    : ICommandHandler<DeleteCustomerCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var customer = await queryRepository.GetEntityByIdAsync(command.Id, cancellationToken);
        if (customer is null)
        {
            return Result.NotFound("مشتری موردنظر پیدا نشد.");
        }

        await commandRepository.DeleteAsync(customer, cancellationToken);
        return true;
    }
}

public sealed class DeleteCustomerCommandValidator : AbstractValidator<DeleteCustomerCommand>
{
    public DeleteCustomerCommandValidator()
    {
        RuleFor(command => command.Id)
            .GreaterThan(0)
            .WithMessage(SharedValidationMessages.GreaterThanZero);
    }
}
