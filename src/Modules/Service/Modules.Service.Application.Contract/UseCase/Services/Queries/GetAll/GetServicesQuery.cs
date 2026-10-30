using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.Services;

namespace Modules.Service.Application.Contract.UseCase.Services.Queries.GetAll;

public sealed record GetServicesQuery(long SalonId, string? SearchTerm = null)
    : IQuery<IReadOnlyList<ServiceDto>>;
