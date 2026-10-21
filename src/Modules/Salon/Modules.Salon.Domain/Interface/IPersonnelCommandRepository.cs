using SharedKernel.Interface.Repositories;

namespace Modules.Personnel.Domain.Interface;

public interface IPersonnelCommandRepository : ICommandRepository<Entities.Personnel>
{
}
