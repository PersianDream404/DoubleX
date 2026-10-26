using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Update;

namespace Modules.Service.Application.UseCase.ServiceCategories.Commands.Update;

public sealed class UpdateServiceCategoryCommandHandler(IServiceCategoryRepository repository)
    : ICommandHandler<UpdateServiceCategoryCommand, bool>
{
    public async Task<Result<bool>> Handle(UpdateServiceCategoryCommand command, CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(
            await repository.UpdateCategoryAsync(command.Id, command.Request, cancellationToken));
}

public sealed class UpdateServiceCategoryCommandValidator : AbstractValidator<UpdateServiceCategoryCommand>
{
    public UpdateServiceCategoryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.Request.SalonId).GreaterThan(0);
        RuleFor(x => x.Request.Name).NotEmpty().Must(name => !string.IsNullOrWhiteSpace(name)).MaximumLength(200);
        RuleFor(x => x.Request.Description).MaximumLength(1000);
    }
}
