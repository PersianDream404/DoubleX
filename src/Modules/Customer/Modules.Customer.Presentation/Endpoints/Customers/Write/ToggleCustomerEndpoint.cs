using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.Get;
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
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var customer = await queryBus.Send<GetCustomerByIdQuery, CustomerDetailsDto>(
                        new GetCustomerByIdQuery(id), cancellationToken);
                    if (!customer.IsSuccess || customer.Value!.SalonId != salonId)
                        return NotFound("مشتری موردنظر پیدا نشد.");

                    var result = await commandBus.Send<ToggleCustomerCommand, bool>(
                        new ToggleCustomerCommand(id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "وضعیت مشتری تغییر کرد." })
                        : NotFound(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
