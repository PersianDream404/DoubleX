using Modules.Banner.Domain.Entities;
using SharedKernel.Interface.Repositories;

namespace Modules.Banner.Domain.Interface
{
    public interface IBannerCommandRepository : ICommandRepository<BannerEntity>
    {
    }
}
