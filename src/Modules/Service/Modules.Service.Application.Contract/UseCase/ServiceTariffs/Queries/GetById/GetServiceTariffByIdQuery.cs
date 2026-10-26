using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;

namespace Modules.Service.Application.Contract.UseCase.ServiceTariffs.Queries.GetById;

public sealed record GetServiceTariffByIdQuery(long Id) : IQuery<ServiceTariffDto>;
