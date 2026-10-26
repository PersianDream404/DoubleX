using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.AddServices;

namespace Modules.Service.Application.UseCase.ServiceTariffs.Commands.AddServices;

public sealed class AddServicesToTariffCommandHandler(IServiceTariffCommandRepository repository)
    : ICommandHandler<AddServicesToTariffCommand, bool>
{
    public async Task<Result<bool>> Handle(
        AddServicesToTariffCommand command,
        CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(
            await repository.AddServicesAsync(command.TariffId, command.Request, cancellationToken));
}

public sealed class AddServicesToTariffCommandValidator : AbstractValidator<AddServicesToTariffCommand>
{
    public AddServicesToTariffCommandValidator()
    {
        RuleFor(command => command.TariffId).GreaterThan(0);
        RuleFor(command => command.Request.SalonId).GreaterThan(0);
        RuleFor(command => command.Request.Services)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(services => services.Select(service => service.ServiceId).Distinct().Count() == services.Count)
            .WithMessage("هر خدمت فقط یک بار می‌تواند در تعرفه ثبت شود.");
        RuleForEach(command => command.Request.Services).ChildRules(service =>
        {
            service.RuleFor(item => item.ServiceId).GreaterThan(0);
            service.RuleFor(item => item.Amount)
                .GreaterThanOrEqualTo(0)
                .LessThanOrEqualTo(9999999999999999.99m);
        });
    }
}
