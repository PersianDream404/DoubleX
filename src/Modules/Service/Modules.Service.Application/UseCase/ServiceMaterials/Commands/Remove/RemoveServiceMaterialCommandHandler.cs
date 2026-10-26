using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceMaterials;
using Modules.Service.Application.Contract.UseCase.ServiceMaterials.Commands.Remove;

namespace Modules.Service.Application.UseCase.ServiceMaterials.Commands.Remove;

public sealed class RemoveServiceMaterialCommandHandler(IServiceMaterialRepository repository)
    : ICommandHandler<RemoveServiceMaterialCommand, bool>
{
    public async Task<Result<bool>> Handle(RemoveServiceMaterialCommand command, CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(await repository.RemoveMaterialAsync(command.SalonId, command.Id, cancellationToken));
}

public sealed class RemoveServiceMaterialCommandValidator : AbstractValidator<RemoveServiceMaterialCommand>
{
    public RemoveServiceMaterialCommandValidator()
    {
        RuleFor(x => x.SalonId).GreaterThan(0);
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
