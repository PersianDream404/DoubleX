using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.UseCase.Customers.Commands.Delete;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation.Endpoints.Customers;

public sealed class DeleteCustomerEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete($"{CustomerApiInfo.Prefix}/{{id:long}}", async (
                    long id,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await commandBus.Send<DeleteCustomerCommand, bool>(
                        new DeleteCustomerCommand(id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "مشتری حذف شد." })
                        : NotFound(result.GetErrorMessage());
                })
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
