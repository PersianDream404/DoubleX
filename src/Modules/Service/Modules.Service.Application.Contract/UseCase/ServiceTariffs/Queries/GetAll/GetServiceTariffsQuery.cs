using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;

namespace Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetAll;

public sealed record GetServiceTariffsQuery(long SalonId) : IQuery<IReadOnlyList<ServiceTariffDto>>;
