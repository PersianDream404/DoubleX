using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.AddServices;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceTariffs.AddServices;

public sealed class AddServicesToTariffEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost($"{ServiceApiInfo.Prefix}/Tariffs/{{tariffId:long}}/Services", async (
                    long tariffId,
                    [FromBody] AddServiceTariffItemsRequestDto request,
                    [FromServices] ICommandBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<AddServicesToTariffCommand, bool>(
                        new AddServicesToTariffCommand(tariffId, request with { SalonId = salonId }), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "خدمات به تعرفه اضافه شدند." })
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}
