using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Application.Contract.UseCase.Services.Commands.Deactivate;

namespace Modules.Service.Application.UseCase.Services.Commands.Deactivate;

public sealed class DeactivateServiceCommandHandler(IServiceCommandRepository repository)
    : ICommandHandler<DeactivateServiceCommand, bool>
{
    public async Task<Result<bool>> Handle(DeactivateServiceCommand command, CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.DeactivateServiceAsync(command.SalonId, command.Id, cancellationToken));
}

public sealed class DeactivateServiceCommandValidator : AbstractValidator<DeactivateServiceCommand>
{
    public DeactivateServiceCommandValidator()
    {
        RuleFor(x => x.SalonId).GreaterThan(0);
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
