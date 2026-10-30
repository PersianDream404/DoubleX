using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.Get;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation.Endpoints.Customers;

public sealed class GetCustomerByIdEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{CustomerApiInfo.Prefix}/{{id:long}}", async (
                    long id,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await queryBus.Send<GetCustomerByIdQuery, CustomerDetailsDto>(
                        new GetCustomerByIdQuery(id), cancellationToken);
                    return result.IsSuccess && result.Value!.SalonId == salonId
                        ? Ok(result.Value)
                        : NotFound(result.IsSuccess ? "مشتری موردنظر پیدا نشد." : result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
