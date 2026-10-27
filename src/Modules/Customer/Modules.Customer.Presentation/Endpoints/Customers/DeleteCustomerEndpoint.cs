using Framwork.Bus.Command;
using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.Get;
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

                    var result = await commandBus.Send<DeleteCustomerCommand, bool>(
                        new DeleteCustomerCommand(id), cancellationToken);
                    return result.IsSuccess
                        ? Ok(new { message = "مشتری حذف شد." })
                        : NotFound(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
