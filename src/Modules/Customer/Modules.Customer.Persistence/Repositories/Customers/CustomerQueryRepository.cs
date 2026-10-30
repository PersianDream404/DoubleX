using Framwork.PagedList;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;
using Modules.Customer.Application.Contract.Interfaces.Customers;
using Modules.Customer.Persistence.Context;
using Modules.Customer.Persistence.Mapper.Customers;
using CustomerEntity = Modules.Customer.Domain.Entities.Customer;

namespace Modules.Customer.Persistence.Repositories.Customers;

public sealed class CustomerQueryRepository(CustomerReadDbContext context)
    : QueryRepository<CustomerEntity>(context), ICustomerQueryRepository
{
    public Task<CustomerEntity?> GetEntityByIdAsync(
        long id,
        CancellationToken cancellationToken) =>
        context.Customers.FirstOrDefaultAsync(customer => customer.Id == id, cancellationToken);

    public Task<bool> SalonExistsAsync(
        long salonId,
        CancellationToken cancellationToken) =>
        context.Set<CustomerSalonReference>()
            .AnyAsync(salon => salon.Id == salonId, cancellationToken);

    public Task<bool> MobilePhoneExistsAsync(
        long salonId,
        string mobilePhone,
        long? excludingCustomerId,
        CancellationToken cancellationToken) =>
        context.Customers.AnyAsync(customer =>
                customer.SalonId == salonId &&
                customer.MobilePhone == mobilePhone &&
                (!excludingCustomerId.HasValue || customer.Id != excludingCustomerId.Value),
            cancellationToken);

    public async Task<PagedList<CustomerListItemDto>> GetAllProjectedAsync(
        GetCustomersRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = context.Customers
            .AsNoTracking()
            .Where(customer => customer.SalonId == request.SalonId);

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var term = request.Q.Trim();
            query = query.Where(customer =>
                customer.FullName.Contains(term) ||
                customer.NationalCode.Contains(term) ||
                customer.MobilePhone.Contains(term));
        }

        return await query
            .OrderBy(customer => customer.FullName)
            .Select(CustomerMapper.ToGetAllDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public async Task<PagedList<GetSelectListCustomerResponseDto>> GetSelectListProjectedAsync(
        GetSelectListCustomerRequestDto request,
        CancellationToken cancellationToken)
    {
        var query = context.Customers
            .AsNoTracking()
            .Where(customer => customer.SalonId == request.SalonId)
            .Where(customer => customer.Status);

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            var term = request.Q.Trim();
            query = query.Where(customer =>
                customer.FullName.Contains(term) ||
                customer.NationalCode.Contains(term) ||
                customer.MobilePhone.Contains(term));
        }

        return await query
            .OrderBy(customer => customer.FullName)
            .Select(CustomerMapper.ToGetSelectListDto())
            .ToPagedList(request.PageNumber, request.PageSize, cancellationToken);
    }

    public async Task<CustomerDetailsDto?> GetByIdProjectedAsync(
        long id,
        CancellationToken cancellationToken)
    {
        return await context.Customers
            .AsNoTracking()
            .Where(customer => customer.Id == id)
            .Select(CustomerMapper.ToGetByIdDto())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CustomerReceptionLookupDto>> GetReceptionLookupsAsync(
        long salonId,
        IReadOnlyCollection<long> customerIds,
        CancellationToken cancellationToken) =>
        await context.Customers
            .AsNoTracking()
            .Where(customer =>
                customer.SalonId == salonId &&
                customerIds.Contains(customer.Id))
            .Select(customer => new CustomerReceptionLookupDto(
                customer.Id,
                customer.FullName,
                customer.MobilePhone,
                customer.Status))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<long>> GetIdsBySearchAsync(
        long salonId,
        string search,
        CancellationToken cancellationToken)
    {
        var term = search.Trim();
        return await context.Customers
            .AsNoTracking()
            .Where(customer =>
                customer.SalonId == salonId &&
                (customer.FullName.Contains(term) ||
                 customer.MobilePhone.Contains(term)))
            .Select(customer => customer.Id)
            .ToListAsync(cancellationToken);
    }
}
