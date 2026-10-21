using Infrastructure.Repositories;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Persistence.Context;

namespace Modules.Personnel.Persistence.Repositories;

public sealed class PersonnelCommandRepository : CommandRepository<Domain.Entities.Personnel>, IPersonnelCommandRepository
{
    public PersonnelCommandRepository(PersonnelWriteDbContext context) : base(context)
    {
    }
}
