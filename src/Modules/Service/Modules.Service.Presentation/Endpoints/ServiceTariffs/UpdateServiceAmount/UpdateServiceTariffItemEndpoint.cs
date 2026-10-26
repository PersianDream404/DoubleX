using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceTariffs;
using Modules.Service.Application.Contract.UseCase.ServiceTariffs.Commands.UpdateServiceAmount;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceTariffs.UpdateServiceAmount;

public sealed class UpdateServiceTariffItemEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPut(
                    $"{ServiceApiInfo.Prefix}/Tariffs/{{tariffId:long}}/Services/{{serviceId:long}}",
                    async (
                        long tariffId,
                        long serviceId,
                        [FromBody] UpdateServiceTariffItemRequestDto request,
                        [FromServices] ICommandBus bus,
                        CancellationToken cancellationToken) =>
                    {
                        var result = await bus.Send<UpdateServiceTariffItemCommand, bool>(
                            new UpdateServiceTariffItemCommand(tariffId, serviceId, request),
                            cancellationToken);
                        return result.IsSuccess
                            ? Ok(new { message = "قیمت خدمت در تعرفه ویرایش شد." })
                            : BadRequest(result.GetErrorMessage());
                    })
                .WithTags(ServiceApiInfo.Tag);
    }
}
