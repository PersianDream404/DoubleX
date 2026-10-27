using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.CashRegister.Application.Contract.Interfaces;
using Modules.CashRegister.Application.Contract.UseCase.CashRegisters.Commands;
using Modules.CashRegister.Domain.Entities;
using CashRegisterEntity = Modules.CashRegister.Domain.Entities.CashRegister;

namespace Modules.CashRegister.Application.UseCase.CashRegisters.Commands;

public sealed class CreateCashRegisterCommandHandler(ICashRegisterCommandRepository repository)
    : ICommandHandler<CreateCashRegisterCommand, long>
{
    public async Task<Result<long>> Handle(CreateCashRegisterCommand command, CancellationToken cancellationToken)
    {
        if (await repository.IsCodeUsedAsync(command.SalonId, command.Request.Code.Trim(), null, cancellationToken))
            return Result.Conflict("کد صندوق در این سالن قبلاً ثبت شده است.");

        var register = CashRegisterEntity.Create(
            command.SalonId,
            command.Request.Name,
            command.Request.Code,
            command.Request.Description);
        return await repository.AddRegisterAsync(register, cancellationToken)
            ? Result.Success(register.Id)
            : Result.Conflict("کد صندوق در این سالن قبلاً ثبت شده است.");
    }
}

public sealed class CreateCashRegisterCommandValidator : AbstractValidator<CreateCashRegisterCommand>
{
    public CreateCashRegisterCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.Name).NotEmpty().MaximumLength(150);
            RuleFor(command => command.Request.Code).NotEmpty().MaximumLength(50)
                .Matches("^[A-Za-z0-9_-]+$");
            RuleFor(command => command.Request.Description).MaximumLength(1000);
        });
    }
}
