using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.DTOs.Customers.Create;
using Modules.Customer.Application.Contract.UseCase.Customers.Commands.Create;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation.Endpoints.Customers;

public sealed class CreateCustomerEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost(CustomerApiInfo.Prefix, async (
                    [FromBody] CreateCustomerRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await commandBus.Send<CreateCustomerCommand, bool>(
                        new CreateCustomerCommand(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "مشتری با موفقیت ثبت شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
