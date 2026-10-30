using Ardalis.Result;
using FluentValidation;
using Framwork.Bus.Query;
using Modules.Personnel.Application.Contract.DTOs.Personnel;
using Modules.Personnel.Application.Contract.Interfaces;
using Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;

namespace Modules.Personnel.Application.UseCase.Personnel.Queries;

public sealed class GetPersonnelReceptionLookupsQueryHandler(
    IPersonnelQueryRepository repository)
    : IQueryHandler<
        GetPersonnelReceptionLookupsQuery,
        IReadOnlyList<PersonnelReceptionLookupDto>>
{
    public async Task<Result<IReadOnlyList<PersonnelReceptionLookupDto>>> Handle(
        GetPersonnelReceptionLookupsQuery query,
        CancellationToken cancellationToken) =>
        Result.Success(await repository.GetReceptionLookupsAsync(
            query.SalonId,
            query.PersonnelIds,
            cancellationToken));
}

public sealed class GetPersonnelReceptionLookupsQueryValidator
    : AbstractValidator<GetPersonnelReceptionLookupsQuery>
{
    public GetPersonnelReceptionLookupsQueryValidator()
    {
        RuleFor(query => query.SalonId).GreaterThan(0);
        RuleFor(query => query.PersonnelIds)
            .NotNull()
            .Must(ids => ids.Count <= 501)
            .WithMessage("تعداد شناسه‌های پرسنل بیش از حد مجاز است.");
        When(query => query.PersonnelIds is not null, () =>
            RuleForEach(query => query.PersonnelIds).GreaterThan(0));
    }
}
