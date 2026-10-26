using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.RemoveService;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceTariffs.RemoveService;

public sealed class RemoveServiceFromTariffEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapDelete(
                    $"{ServiceApiInfo.Prefix}/Tariffs/{{tariffId:long}}/Services/{{serviceId:long}}",
                    async (
                        long tariffId,
                        long serviceId,
                        [FromQuery] long salonId,
                        [FromServices] ICommandBus bus,
                        CancellationToken cancellationToken) =>
                    {
                        var result = await bus.Send<RemoveServiceFromTariffCommand, bool>(
                            new RemoveServiceFromTariffCommand(tariffId, serviceId, salonId),
                            cancellationToken);
                        return result.IsSuccess
                            ? Ok(new { message = "خدمت از تعرفه حذف شد." })
                            : BadRequest(result.GetErrorMessage());
                    })
                .WithTags(ServiceApiInfo.Tag);
    }
}
