using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServicePrices;
using Modules.Service.Application.Contract.UseCase.ServicePrices.Commands.Create;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServicePrices.Create;

public sealed class AddServicePriceEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost($"{ServiceApiInfo.Prefix}/Prices", async (
                    [FromBody] CreateServicePriceRequestDto request,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<AddServicePriceCommand, bool>(
                        new AddServicePriceCommand(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "تعرفه جدید ثبت شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
