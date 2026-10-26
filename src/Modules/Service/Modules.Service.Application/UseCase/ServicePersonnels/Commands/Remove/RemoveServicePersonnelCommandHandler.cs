using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Commands.Remove;

namespace Modules.Service.Application.UseCase.ServicePersonnels.Commands.Remove;

public sealed class RemoveServicePersonnelCommandHandler(
    IServicePersonnelCommandRepository repository)
    : ICommandHandler<RemoveServicePersonnelCommand, bool>
{
    public async Task<Result<bool>> Handle(
        RemoveServicePersonnelCommand command,
        CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.RemovePersonnelAsync(
            command.SalonId,
            command.ServiceId,
            command.Id,
            cancellationToken));
}

public sealed class RemoveServicePersonnelCommandValidator
    : AbstractValidator<RemoveServicePersonnelCommand>
{
    public RemoveServicePersonnelCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.ServiceId).GreaterThan(0);
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
