using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;
using Modules.Service.Application.Contract.Interfaces.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Commands.Upsert;

namespace Modules.Service.Application.UseCase.ServicePersonnels.Commands.Upsert;

public sealed class UpsertServicePersonnelCommandHandler(
    IServicePersonnelCommandRepository repository,
    IQueryBus queryBus)
    : ICommandHandler<UpsertServicePersonnelCommand, bool>
{
    public async Task<Result<bool>> Handle(
        UpsertServicePersonnelCommand command,
        CancellationToken cancellationToken)
    {
        var personnelResult = await queryBus.Send<GetPersonnelByIdQuery, GetByIdPersonnelResponseDto>(
            new GetPersonnelByIdQuery(command.Request.PersonnelId),
            cancellationToken);
        if (!personnelResult.IsSuccess ||
            !personnelResult.Value.Status ||
            personnelResult.Value.SalonId != command.Request.SalonId)
        {
            return ServiceCatalogResult.ToResult(
                Modules.Service.Application.Contract.Enums.ServiceOperationStatus.InvalidPersonnel);
        }

        return ServiceCatalogResult.ToResult(await repository.UpsertPersonnelAsync(
            command.Id,
            command.Request,
            cancellationToken));
    }
}

public sealed class UpsertServicePersonnelCommandValidator
    : AbstractValidator<UpsertServicePersonnelCommand>
{
    public UpsertServicePersonnelCommandValidator()
    {
        RuleFor(command => command.Id).GreaterThan(0).When(command => command.Id.HasValue);
        RuleFor(command => command.Request).NotNull();
        When(command => command.Request is not null, () =>
        {
            RuleFor(command => command.Request.SalonId).GreaterThan(0);
            RuleFor(command => command.Request.ServiceId).GreaterThan(0);
            RuleFor(command => command.Request.PersonnelId).GreaterThan(0);
        });
    }
}
