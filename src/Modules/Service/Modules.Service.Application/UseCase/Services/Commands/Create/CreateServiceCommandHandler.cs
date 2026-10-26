using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.Services;
using Modules.Service.Application.Contract.UseCase.Services.Commands.Create;

namespace Modules.Service.Application.UseCase.Services.Commands.Create;

public sealed class CreateServiceCommandHandler(IServiceRepository repository)
    : ICommandHandler<CreateServiceCommand, bool>
{
    public async Task<Result<bool>> Handle(CreateServiceCommand command, CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.CreateServiceAsync(command.Request, cancellationToken));
}

public sealed class CreateServiceCommandValidator : AbstractValidator<CreateServiceCommand>
{
    public CreateServiceCommandValidator()
    {
        RuleFor(x => x.Request.SalonId).GreaterThan(0);
        RuleFor(x => x.Request.CategoryId).GreaterThan(0);
        RuleFor(x => x.Request.Code).NotEmpty().Must(code => !string.IsNullOrWhiteSpace(code)).MaximumLength(50);
        RuleFor(x => x.Request.Name).NotEmpty().Must(name => !string.IsNullOrWhiteSpace(name)).MaximumLength(200);
        RuleFor(x => x.Request.Description).MaximumLength(1000);
        RuleFor(x => x.Request.DurationMinutes).GreaterThan(0);
    }
}
