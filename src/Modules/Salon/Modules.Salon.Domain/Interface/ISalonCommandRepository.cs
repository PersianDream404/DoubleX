
using SharedKernel.Interface.Repositories;

namespace Modules.Salon.Domain.Interface
{
    public interface ISalonCommandRepository : ICommandRepository<Personnel.Domain.Entities.Salon>
    {
    }
}
