using Framwork.PagedList;
using Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Modules.Reception.Domain.Entities;
using Modules.Reception.Domain.Interfaces;
using Modules.Reception.Domain.ReadModels;
using Modules.Reception.Persistence.Context;
using Modules.Reception.Persistence.Mapper;

namespace Modules.Reception.Persistence.Repositories;

public sealed class ReceptionQueryRepository(ReceptionReadDbContext context)
    : IReceptionQueryRepository
{
    public async Task<PagedList<ReceptionListReadModel>> GetAllAsync(
        long salonId,
        long? customerId,
        bool unpaidOnly,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = context.Receptions
            .AsNoTracking()
            .Where(item => item.SalonId == salonId);
        if (customerId.HasValue)
        {
            query = query.Where(item => item.CustomerId == customerId.Value);
        }
        if (unpaidOnly)
        {
            query = query.Where(item => item.TotalAmount >
                (item.Payments
                    .Where(payment => payment.PaymentStatus == ReceptionPaymentStatus.Succeeded)
                    .Sum(payment => (decimal?)payment.Amount) ?? 0m));
        }

        return await query
            .OrderByDescending(item => item.ReceptionDate)
            .Select(ReceptionMapper.ToListReadModel())
            .ToPagedList(pageNumber, pageSize, cancellationToken);
    }

    public Task<ReceptionDetailsReadModel?> GetByIdAsync(
        long id,
        long salonId,
        CancellationToken cancellationToken) =>
        context.Receptions
            .AsNoTracking()
            .Where(item => item.Id == id && item.SalonId == salonId)
            .Select(ReceptionMapper.ToDetailsReadModel())
            .SingleOrDefaultAsync(cancellationToken);
}
