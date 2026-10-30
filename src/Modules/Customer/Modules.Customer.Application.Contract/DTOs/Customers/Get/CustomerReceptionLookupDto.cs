namespace Modules.Customer.Application.Contract.DTOs.Customers.Get;

public sealed record CustomerReceptionLookupDto(
    long Id,
    string FullName,
    string MobilePhone,
    bool Status);
