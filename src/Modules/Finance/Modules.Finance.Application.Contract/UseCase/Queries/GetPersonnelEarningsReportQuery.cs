using Framwork.Bus.Query;
using Modules.Finance.Application.Contract.DTOs;

namespace Modules.Finance.Application.Contract.UseCase.Queries;

public sealed record GetPersonnelEarningsReportQuery(
    long SalonId,
    long PersonnelId,
    DateTimeOffset? From,
    DateTimeOffset? To) : IQuery<PersonnelEarningsReportDto>;
