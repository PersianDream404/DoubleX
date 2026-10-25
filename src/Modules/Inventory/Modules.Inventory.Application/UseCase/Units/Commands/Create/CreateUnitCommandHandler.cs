using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Inventory.Application.Contract.DTOs.Units;
using Modules.Inventory.Application.Contract.UseCase;
using Modules.Inventory.Domain.Entities;
using Modules.Inventory.Domain.Enums;
using Modules.Inventory.Domain.Interfaces.Units;

namespace Modules.Inventory.Application.UseCase;

public sealed class CreateUnitCommandHandler(IUnitCommandRepository repository)
    : ICommandHandler<CreateUnitCommand, bool>
{
    public async Task<Result<bool>> Handle(CreateUnitCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;
        var unit = Unit.Create(request.Name, request.Symbol, request.UnitType);
        var status = await repository.AddAsync(unit, cancellationToken);
        return status switch
        {
            InventoryOperationStatus.Succeeded => Result.Success(true),
            InventoryOperationStatus.Conflict => Result.Error("واحدی با این نام یا نماد از قبل وجود دارد."),
            _ => Result.Error("ثبت واحد اندازه‌گیری انجام نشد.")
        };
    }
}

public sealed class CreateUnitCommandValidator : AbstractValidator<CreateUnitCommand>
{
    public CreateUnitCommandValidator()
    {
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.Name).NotEmpty().MaximumLength(100);
            RuleFor(command => command.Request.Symbol).NotEmpty().MaximumLength(20);
            RuleFor(command => command.Request.UnitType).IsInEnum();
        });
    }
}
