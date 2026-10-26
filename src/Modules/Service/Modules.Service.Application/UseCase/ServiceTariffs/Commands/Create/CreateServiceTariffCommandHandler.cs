using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.Create;

namespace Modules.Service.Application.UseCase.ServiceTariffs.Commands.Create;

public sealed class CreateServiceTariffCommandHandler(IServiceTariffCommandRepository repository)
    : ICommandHandler<CreateServiceTariffCommand, bool>
{
    public async Task<Result<bool>> Handle(
        CreateServiceTariffCommand command,
        CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.CreateTariffAsync(command.Request, cancellationToken));
}

public sealed class CreateServiceTariffCommandValidator : AbstractValidator<CreateServiceTariffCommand>
{
    public CreateServiceTariffCommandValidator()
    {
        RuleFor(command => command.Request.SalonId).GreaterThan(0);
        RuleFor(command => command.Request.Name)
            .NotEmpty()
            .Must(name => !string.IsNullOrWhiteSpace(name))
            .MaximumLength(200);
        RuleFor(command => command.Request.ValidFrom).NotEmpty();
        RuleFor(command => command.Request.ValidTo)
            .Must((command, validTo) =>
                !validTo.HasValue || validTo.Value > command.Request.ValidFrom)
            .WithMessage("پایان اعتبار تعرفه باید بعد از شروع اعتبار باشد.");
    }
}
