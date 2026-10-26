using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.RemoveService;

namespace Modules.Service.Application.UseCase.ServiceTariffs.Commands.RemoveService;

public sealed class RemoveServiceFromTariffCommandHandler(IServiceTariffCommandRepository repository)
    : ICommandHandler<RemoveServiceFromTariffCommand, bool>
{
    public async Task<Result<bool>> Handle(
        RemoveServiceFromTariffCommand command,
        CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.RemoveServiceAsync(
            command.TariffId,
            command.ServiceId,
            command.SalonId,
            cancellationToken));
}

public sealed class RemoveServiceFromTariffCommandValidator : AbstractValidator<RemoveServiceFromTariffCommand>
{
    public RemoveServiceFromTariffCommandValidator()
    {
        RuleFor(command => command.TariffId).GreaterThan(0);
        RuleFor(command => command.ServiceId).GreaterThan(0);
        RuleFor(command => command.SalonId).GreaterThan(0);
    }
}
