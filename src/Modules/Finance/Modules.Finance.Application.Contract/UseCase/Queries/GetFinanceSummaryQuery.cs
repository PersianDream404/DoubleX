using Framwork.Bus.Query;
using Modules.Finance.Application.Contract.DTOs;

namespace Modules.Finance.Application.Contract.UseCase.Queries;

public sealed record GetFinanceSummaryQuery(
    long SalonId,
    DateTimeOffset? From,
    DateTimeOffset? To) : IQuery<FinanceSummaryDto>;
