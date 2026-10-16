using Framwork.Bus.Query;
using Framwork.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Modules.Banner.Application.Contract.DTOs.Banners.Get;
using Modules.Banner.Application.Contract.UseCase.Banners.Queries;
using Modules.Banner.Domain.Enums;
using SharedKernel.Helper;
using SharedKernel.Interface;

namespace Modules.Banner.Presentation.Endpoints.Banners.Read;

public static class GetSelectListBannerTypeEndpoint
{
    public class EndPoint : BaseEndpoint, IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet($"{ApiInfo.Prefix}/SelectList/Type", handler: async (

                ) =>
            {

                var result = EnumHelper.GetItems<BannerType>();
                return Ok(result);


            })
                .WithTags(ApiInfo.Tag);
        }
    }
}