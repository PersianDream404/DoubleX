using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.UpdateServiceAmount;

namespace Modules.Service.Application.UseCase.ServiceTariffs.Commands.UpdateServiceAmount;

public sealed class UpdateServiceTariffItemCommandHandler(IServiceTariffCommandRepository repository)
    : ICommandHandler<UpdateServiceTariffItemCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateServiceTariffItemCommand command,
        CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.UpdateServiceAmountAsync(
            command.TariffId,
            command.ServiceId,
            command.Request,
            cancellationToken));
}

public sealed class UpdateServiceTariffItemCommandValidator : AbstractValidator<UpdateServiceTariffItemCommand>
{
    public UpdateServiceTariffItemCommandValidator()
    {
        RuleFor(command => command.TariffId).GreaterThan(0);
        RuleFor(command => command.ServiceId).GreaterThan(0);
        RuleFor(command => command.Request.SalonId).GreaterThan(0);
        RuleFor(command => command.Request.Amount)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(9999999999999999.99m);
    }
}
