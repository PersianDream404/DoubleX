using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Commands.Create;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceCategories.Create;

public sealed class CreateServiceCategoryEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapPost($"{ServiceApiInfo.Prefix}/Categories", async (
                    [FromBody] ServiceCategoryRequestDto request,
                    [FromServices] ICommandBus bus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await bus.Send<CreateServiceCategoryCommand, bool>(
                        new CreateServiceCategoryCommand(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "دسته‌بندی خدمت ثبت شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(ServiceApiInfo.Tag);
    }
}
