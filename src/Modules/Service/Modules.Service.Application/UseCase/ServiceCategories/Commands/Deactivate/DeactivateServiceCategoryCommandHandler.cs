using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Deactivate;

namespace Modules.Service.Application.UseCase.ServiceCategories.Commands.Deactivate;

public sealed class DeactivateServiceCategoryCommandHandler(IServiceCategoryRepository repository)
    : ICommandHandler<DeactivateServiceCategoryCommand, bool>
{
    public async Task<Result<bool>> Handle(DeactivateServiceCategoryCommand command, CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(
            await repository.DeactivateCategoryAsync(command.SalonId, command.Id, cancellationToken));
}

public sealed class DeactivateServiceCategoryCommandValidator : AbstractValidator<DeactivateServiceCategoryCommand>
{
    public DeactivateServiceCategoryCommandValidator()
    {
        RuleFor(x => x.SalonId).GreaterThan(0);
        RuleFor(x => x.Id).GreaterThan(0);
    }
}
