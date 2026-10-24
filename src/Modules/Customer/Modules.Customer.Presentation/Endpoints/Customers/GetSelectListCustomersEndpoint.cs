using Framwork.Bus.Query;
using Framwork.Extensions;
using Framwork.PagedList;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;
using Modules.Customer.Application.Contract.UseCase.Customers.Queries.GetSelectList;
using SharedKernel.Interface;

namespace Modules.Customer.Presentation.Endpoints.Customers;

public sealed class GetSelectListCustomersEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{CustomerApiInfo.Prefix}/SelectList", async (
                    [AsParameters] GetSelectListCustomerRequestDto request,
                    [FromServices] IQueryBus queryBus,
                    CancellationToken cancellationToken) =>
                {
                    var result = await queryBus.Send<
                        GetSelectListCustomerQuery,
                        PagedList<GetSelectListCustomerResponseDto>>(
                        new GetSelectListCustomerQuery(request),
                        cancellationToken);

                    return result.IsSuccess
                        ? Ok(result.Value)
                        : BadRequest(result.GetErrorMessage());
                })
                .WithTags(CustomerApiInfo.Tag);
        }
    }
}
