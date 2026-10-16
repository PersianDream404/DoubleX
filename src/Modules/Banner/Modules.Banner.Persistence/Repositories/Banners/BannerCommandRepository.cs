using Infrastructure.Repositories;
using Modules.Banner.Domain.Entities;
using Modules.Banner.Domain.Interface;
using Modules.Banner.Persistence.Context;
namespace Modules.Banner.Persistence.Repositories.Banners;


public class BannerCommandRepository
    : CommandRepository<BannerEntity>, IBannerCommandRepository
{
    public BannerCommandRepository(BannerWriteDbContext context) : base(context)
    {
    }

 
}
