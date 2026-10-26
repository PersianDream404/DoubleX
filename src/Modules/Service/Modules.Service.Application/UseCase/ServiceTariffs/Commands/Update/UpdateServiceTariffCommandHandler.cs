using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.Update;

namespace Modules.Service.Application.UseCase.ServiceTariffs.Commands.Update;

public sealed class UpdateServiceTariffCommandHandler(IServiceTariffCommandRepository repository)
    : ICommandHandler<UpdateServiceTariffCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpdateServiceTariffCommand command,
        CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(
            await repository.UpdateTariffAsync(command.Id, command.Request, cancellationToken));
}

public sealed class UpdateServiceTariffCommandValidator : AbstractValidator<UpdateServiceTariffCommand>
{
    public UpdateServiceTariffCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0);
        RuleFor(command => command.Request.SalonId).GreaterThan(0);
        RuleFor(command => command.Request.Name)
            .NotEmpty()
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .MaximumLength(200);
        RuleFor(command => command.Request.ValidFrom).NotEmpty();
        RuleFor(command => command.Request.ValidTo)
            .Must((command, validTo) => !validTo.HasValue || validTo.Value > command.Request.ValidFrom)
            .WithMessage("پایان اعتبار تعرفه باید بعد از شروع اعتبار باشد.");
    }
}
