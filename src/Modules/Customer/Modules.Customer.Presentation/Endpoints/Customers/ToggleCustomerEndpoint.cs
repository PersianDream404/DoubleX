using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.UseCase.Customers.Commands.Toggle;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation.Endpoints.Customers;

public sealed class ToggleCustomerEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{CustomerApiInfo.Prefix}/{{id:long}}/Toggle", async (
                    long id,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await commandBus.Send<ToggleCustomerCommand, bool>(
                        new ToggleCustomerCommand(id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "وضعیت مشتری تغییر کرد." })
                        : NotFound(result.GetErrorMessage());
                })
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
