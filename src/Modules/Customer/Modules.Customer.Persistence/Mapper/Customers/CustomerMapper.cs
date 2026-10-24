using System.Linq.Expressions;
using Modules.Customer.Application.Contract.DTOs.Customers.Get;
using Modules.Customer.Application.Contract.DTOs.Customers.GetAll;
using CustomerEntity = Modules.Customer.Domain.Entities.Customer;

namespace Modules.Customer.Persistence.Mapper.Customers;

public static class CustomerMapper
{
    public static Expression<Func<CustomerEntity, CustomerListItemDto>> ToGetAllDto() =>
        customer => new CustomerListItemDto
        {
            Id = customer.Id,
            SalonId = customer.SalonId,
            FullName = customer.FullName,
            BirthDate = customer.BirthDate,
            NationalCode = customer.NationalCode,
            Address = customer.Address,
            AllergyDescription = customer.AllergyDescription,
            JobTitle = customer.JobTitle,
            LandlinePhone = customer.LandlinePhone,
            MobilePhone = customer.MobilePhone,
            Status = customer.Status
        };

    public static Expression<Func<CustomerEntity, CustomerDetailsDto>> ToGetByIdDto() =>
        customer => new CustomerDetailsDto
        {
            Id = customer.Id,
            SalonId = customer.SalonId,
            FullName = customer.FullName,
            BirthDate = customer.BirthDate,
            NationalCode = customer.NationalCode,
            Address = customer.Address,
            AllergyDescription = customer.AllergyDescription,
            JobTitle = customer.JobTitle,
            LandlinePhone = customer.LandlinePhone,
            MobilePhone = customer.MobilePhone,
            Status = customer.Status
        };
}
