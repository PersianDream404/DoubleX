using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.Services.Queries.GetById;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.Services.GetById;

public sealed class GetServiceByIdEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/{{id:long}}", async (
                    long id,
                    IQueryBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<GetServiceByIdQuery, ServiceDto>(
                        new GetServiceByIdQuery(id), cancellationToken);
                    return result.IsSuccess && result.Value!.SalonId == salonId
                        ? Ok(result.Value)
                        : NotFound(result.IsSuccess ? "خدمت یافت نشد." : result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}
