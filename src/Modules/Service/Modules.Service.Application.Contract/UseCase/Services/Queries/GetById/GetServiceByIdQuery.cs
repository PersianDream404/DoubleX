using Framwork.Bus.Query;
using Modules.Service.Application.Contract.DTOs.Services;

namespace Modules.Service.Application.Contract.UseCase.Services.Queries.GetById;

public sealed record GetServiceByIdQuery(long Id) : IQuery<ServiceDto>;
