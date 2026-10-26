using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Delete;

namespace Modules.Service.Application.UseCase.ServiceCategories.Commands.Delete;

public sealed class DeleteServiceCategoryCommandHandler(IServiceCategoryCommandRepository repository)
    : ICommandHandler<DeleteServiceCategoryCommand, bool>
{
    public async Task<Result<bool>> Handle(
        DeleteServiceCategoryCommand command,
        CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(
            await repository.DeleteCategoryAsync(command.SalonId, command.Id, cancellationToken));
}

public sealed class DeleteServiceCategoryCommandValidator : AbstractValidator<DeleteServiceCategoryCommand>
{
    public DeleteServiceCategoryCommandValidator()
    {
        RuleFor(command => command.SalonId).GreaterThan(0);
        RuleFor(command => command.Id).GreaterThan(0);
    }
}
