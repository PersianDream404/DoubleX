using SharedKernel.Interface.Repositories;

namespace Modules.Personnel.Domain.Interface;

public interface IPersonnelCommandRepository : ICommandRepository<Entities.Personnel>
{
    Task<bool> UpdateWithContractAsync(
        Entities.Personnel personnel,
        Entities.PersonnelContract contract,
        CancellationToken cancellationToken);
}
