using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using Modules.Personnel.Domain.Entities;
using Modules.Personnel.Domain.Interface;
using Modules.Personnel.Persistence.Context;

namespace Modules.Personnel.Persistence.Repositories;

public sealed class PersonnelCommandRepository : CommandRepository<Domain.Entities.Personnel>, IPersonnelCommandRepository
{
    private readonly PersonnelWriteDbContext _personnelContext;

    public PersonnelCommandRepository(PersonnelWriteDbContext context) : base(context)
    {
        _personnelContext = context;
    }

    public async Task<bool> UpdateWithContractAsync(
        Domain.Entities.Personnel personnel,
        PersonnelContract contract,
        CancellationToken cancellationToken)
    {
        await using var transaction = await _personnelContext.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);

        var existingContracts = await _personnelContext.PersonnelContracts
            .Where(item => item.PersonnelId == personnel.Id)
            .OrderBy(item => item.EffectiveFrom)
            .ToListAsync(cancellationToken);

        var activeContract = existingContracts.SingleOrDefault(item => item.IsActive);
        if (activeContract is not null && HasSameTerms(activeContract, contract))
        {
            _personnelContext.Personnels.Update(personnel);
            await _personnelContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }

        if (activeContract is not null)
        {
            if (contract.EffectiveFrom <= activeContract.EffectiveFrom ||
                !activeContract.Close(contract.EffectiveFrom.AddDays(-1)))
            {
                return false;
            }
        }

        if (existingContracts.Any(item => Overlaps(item, contract)))
        {
            return false;
        }

        _personnelContext.Personnels.Update(personnel);
        await _personnelContext.PersonnelContracts.AddAsync(contract, cancellationToken);
        try
        {
            await _personnelContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            _personnelContext.ChangeTracker.Clear();
            return false;
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            _personnelContext.ChangeTracker.Clear();
            return false;
        }
    }

    private static bool HasSameTerms(PersonnelContract current, PersonnelContract next) =>
        current.CooperationType == next.CooperationType &&
        current.FixedSalaryAmount == next.FixedSalaryAmount &&
        current.DefaultCommissionRate == next.DefaultCommissionRate &&
        current.EffectiveFrom == next.EffectiveFrom &&
        current.EffectiveTo == next.EffectiveTo;

    private static bool Overlaps(PersonnelContract existing, PersonnelContract next)
    {
        var existingEnd = existing.EffectiveTo ?? DateOnly.MaxValue;
        var nextEnd = next.EffectiveTo ?? DateOnly.MaxValue;
        return next.EffectiveFrom <= existingEnd && existing.EffectiveFrom <= nextEnd;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };
}
