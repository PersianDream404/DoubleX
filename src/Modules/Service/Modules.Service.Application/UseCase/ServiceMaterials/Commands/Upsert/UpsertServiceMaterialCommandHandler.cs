using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceMaterials;
using Modules.Service.Application.Contract.UseCase.ServiceMaterials.Commands.Upsert;

namespace Modules.Service.Application.UseCase.ServiceMaterials.Commands.Upsert;

public sealed class UpsertServiceMaterialCommandHandler(IServiceMaterialRepository repository)
    : ICommandHandler<UpsertServiceMaterialCommand, bool>
{
    public async Task<Result<bool>> Handle(UpsertServiceMaterialCommand command, CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.UpsertMaterialAsync(command.Id, command.Request, cancellationToken));
}

public sealed class UpsertServiceMaterialCommandValidator : AbstractValidator<UpsertServiceMaterialCommand>
{
    public UpsertServiceMaterialCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).When(x => x.Id.HasValue);
        RuleFor(x => x.Request.SalonId).GreaterThan(0);
        RuleFor(x => x.Request.ServiceId).GreaterThan(0);
        RuleFor(x => x.Request.MaterialId).GreaterThan(0);
        RuleFor(x => x.Request.QuantityPerService)
            .GreaterThan(0)
            .LessThanOrEqualTo(999999999999999.999m)
            .Must(value => decimal.Round(value, 3) == value);
        RuleFor(x => x.Request.Description).MaximumLength(1000);
    }
}
