using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.GetAll;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation.Endpoints.Customers;

public sealed class GetCustomersEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(CustomerApiInfo.Prefix, async (
                    [AsParameters] GetCustomersRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();
                    request.SalonId = salonId;

                    var result = await queryBus.Send<GetCustomersQuery,
                        Framwork.PagedList.PagedList<CustomerListItemDto>>(
                        new GetCustomersQuery(request), cancellationToken);
                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
