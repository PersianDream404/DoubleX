using Infrastructure.Repositories;
using Modules.Personnel.Persistence.Context;
using Modules.Salon.Domain.Interface;
namespace Modules.Salon.Persistence.Repositories.Salons;


public class SalonCommandRepository
    : CommandRepository<Personnel.Domain.Entities.Salon>, ISalonCommandRepository
{
    public SalonCommandRepository(PersonnelWriteDbContext context) : base(context)
    {
    }

 
}
