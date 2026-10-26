using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.Services;

namespace Modules.Service.Application.Contract.UseCase.Services.Queries.GetActive;

public sealed record GetActiveServicesQuery(long SalonId) : IQuery<IReadOnlyList<ServiceDto>>;
