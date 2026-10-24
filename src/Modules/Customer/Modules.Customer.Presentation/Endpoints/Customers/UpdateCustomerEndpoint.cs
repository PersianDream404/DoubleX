using Framwork.Bus.Command;
using Framwork.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.DTOs.Customers.Update;
using Modules.Customer.Application.Contract.UseCase.Customers.Commands.Update;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation.Endpoints.Customers;

public sealed class UpdateCustomerEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPut($"{CustomerApiInfo.Prefix}/{{id:long}}", async (
                    long id,
                    [FromBody] UpdateCustomerRequestDto request,
                    [FromServices] ICommandBus commandBus,
                    CancellationToken cancellationToken) =>
                {
                    if (id != request.Id)
                    {
                        return BadRequest("شناسهٔ مسیر و درخواست یکسان نیستند.");
                    }

                    var result = await commandBus.Send<UpdateCustomerCommand, bool>(
                        new UpdateCustomerCommand(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "اطلاعات مشتری به‌روزرسانی شد." })
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
