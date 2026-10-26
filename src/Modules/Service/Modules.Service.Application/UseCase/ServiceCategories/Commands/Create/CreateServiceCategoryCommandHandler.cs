using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Modules.Service.Application.Contract.Interfaces.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Create;

namespace Modules.Service.Application.UseCase.ServiceCategories.Commands.Create;

public sealed class CreateServiceCategoryCommandHandler(IServiceCategoryRepository repository)
    : ICommandHandler<CreateServiceCategoryCommand, bool>
{
    public async Task<Result<bool>> Handle(CreateServiceCategoryCommand command, CancellationToken cancellationToken) =>
        ServiceCatalogResult.ToResult(
            await repository.CreateCategoryAsync(command.Request, cancellationToken));
}

public sealed class CreateServiceCategoryCommandValidator : AbstractValidator<CreateServiceCategoryCommand>
{
    public CreateServiceCategoryCommandValidator()
    {
        RuleFor(x => x.Request.SalonId).GreaterThan(0);
        RuleFor(x => x.Request.Name).NotEmpty().Must(name => !string.IsNullOrWhiteSpace(name)).MaximumLength(200);
        RuleFor(x => x.Request.Description).MaximumLength(1000);
    }
}
