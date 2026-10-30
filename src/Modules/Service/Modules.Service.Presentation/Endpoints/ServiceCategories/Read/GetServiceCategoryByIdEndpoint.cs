using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Service.Application.Contract.DTOs.ServiceCategories;
using Modules.Service.Application.Contract.UseCase.ServiceCategories.Queries.GetById;
using SharedKernel.Interface;

namespace Modules.Service.Presentation.Endpoints.ServiceCategories.GetById;

public sealed class GetServiceCategoryByIdEndpoint
{
    public sealed class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app) =>
            app.MapGet($"{ServiceApiInfo.Prefix}/Categories/{{id:long}}", async (
                    long id,
                    IQueryBus bus,
                    HttpContext context,
                    CancellationToken cancellationToken) =>
                {
                    if (!TryGetSalonId(context, out var salonId))
                        return Unauthorized();

                    var result = await bus.Send<GetServiceCategoryByIdQuery, ServiceCategoryDto>(
                        new GetServiceCategoryByIdQuery(id), cancellationToken);
                    return result.IsSuccess && result.Value!.SalonId == salonId
                        ? Ok(result.Value)
                        : NotFound(result.IsSuccess ? "دسته‌بندی خدمت یافت نشد." : result.GetErrorMessage());
                })
                .RequireAuthorization()
                .WithTags(ServiceApiInfo.Tag);
    }
}
