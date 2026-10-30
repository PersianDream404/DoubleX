using Framwork.Bus.Query;
using Modules.Personnel.Application.Contract.DTOs.Personnel;

namespace Modules.Personnel.Application.Contract.UseCase.Personnel.Queries;

public sealed record GetPersonnelReceptionLookupsQuery(
    long SalonId,
    IReadOnlyCollection<long> PersonnelIds)
    : IQuery<IReadOnlyList<PersonnelReceptionLookupDto>>;
