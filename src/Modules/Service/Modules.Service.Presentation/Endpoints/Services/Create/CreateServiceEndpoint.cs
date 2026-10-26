using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.Services;
using Modules.Service.Application.Contract.UseCase.Services.Commands.Create;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.Services.Create;

public sealed class CreateServiceEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost(ServiceApiInfo.Prefix, async (
                    [FromBody] ServiceRequestDto request,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<CreateServiceCommand, bool>(
                        new CreateServiceCommand(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "خدمت ثبت شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
