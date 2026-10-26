using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.Delete;

namespace Modules.Service.Application.UseCase.ServiceTariffs.Commands.Delete;

public sealed class DeleteServiceTariffCommandHandler(IServiceTariffCommandRepository repository)
    : ICommandHandler<DeleteServiceTariffCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteServiceTariffCommand command,
        CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(
            await repository.DeleteTariffAsync(command.Id, command.SalonId, cancellationToken));
}

public sealed class DeleteServiceTariffCommandValidator : AbstractValidator<DeleteServiceTariffCommand>
{
    public DeleteServiceTariffCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
    }
}
