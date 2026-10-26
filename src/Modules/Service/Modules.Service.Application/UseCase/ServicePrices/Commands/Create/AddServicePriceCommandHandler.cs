using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServicePrices;
using Modules.Service.Application.Contract.UseCase.ServicePrices.Commands.Create;

namespace Modules.Service.Application.UseCase.ServicePrices.Commands.Create;

public sealed class AddServicePriceCommandHandler(IServicePriceRepository repository)
    : ICommandHandler<AddServicePriceCommand, bool>
{
    public async Task<Result<bool>> Handle(AddServicePriceCommand command, CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.AddPriceAsync(command.Request, cancellationToken));
}

public sealed class AddServicePriceCommandValidator : AbstractValidator<AddServicePriceCommand>
{
    public AddServicePriceCommandValidator()
    {
        RuleFor(x => x.Request.SalonId).GreaterThan(0);
        RuleFor(x => x.Request.ServiceId).GreaterThan(0);
        RuleFor(x => x.Request.Amount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(9999999999999999.99m);
        RuleFor(x => x.Request.ValidFrom).NotEmpty();
        RuleFor(x => x.Request.ValidTo)
            .Must((command, validTo) =>
                !validTo.HasValue || validTo.Value > command.Request.ValidFrom)
            .WithMessage("پایان اعتبار باید بعد از شروع اعتبار باشد.");
    }
}
