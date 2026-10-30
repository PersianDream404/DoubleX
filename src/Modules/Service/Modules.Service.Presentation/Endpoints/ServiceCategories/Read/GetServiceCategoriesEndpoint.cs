using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Queries.GetAll;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceCategories.GetAll;

public sealed class GetServiceCategoriesEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/Categories", async (
                    [FromServices] IQueryBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<GetServiceCategoriesQuery, IReadOnlyList<ServiceCategoryDto>>(
                        new GetServiceCategoriesQuery(salonId), cancellationToken);
                    return result.IsSuccess ? Ok(result.Value!) : NotFound(result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}
