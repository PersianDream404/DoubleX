using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServicePersonnels;
using Modules.Service.Application.Contract.UseCase.ServicePersonnels.Queries.GetByPersonnel;
using Modules.Service.Domain.Interfaces;

namespace Modules.Service.Application.UseCase.ServicePersonnels.Queries.GetByPersonnel;

public sealed class GetServicePersonnelAssignmentsQueryHandler(
    IServicePersonnelQueryRepository repository)
    : IQueryHandler<GetServicePersonnelAssignmentsQuery, IReadOnlyList<ServicePersonnelDto>>
{
    public async Task<Result<IReadOnlyList<ServicePersonnelDto>>> Handle(
        GetServicePersonnelAssignmentsQuery query,
        CancellationToken cancellationToken)
    {
        var items = await repository.GetByPersonnelAsync(
            query.SalonId,
            query.PersonnelId,
            cancellationToken);
        return Result.Success<IReadOnlyList<ServicePersonnelDto>>(items
            .Select(item => new ServicePersonnelDto(
                item.Id,
                item.SalonId,
                item.ServiceId,
                item.PersonnelId))
            .ToList());
    }
}

public sealed class GetServicePersonnelAssignmentsQueryValidator
    : AbstractValidator<GetServicePersonnelAssignmentsQuery>
{
    public GetServicePersonnelAssignmentsQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.PersonnelId).GreaterThan(0);
    }
}
