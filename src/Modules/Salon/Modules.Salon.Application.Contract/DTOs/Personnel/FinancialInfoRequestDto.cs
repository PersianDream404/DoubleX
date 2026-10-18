using Modules.Personnel.Domain.Enums;

namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed class FinancialInfoRequestDto
{
    public SalaryType SalaryType { get; set; }
    public string CardNumber { get; set; }
    public string AccountNumber { get; set; }

    public FinancialInfoRequestDto(
        SalaryType salaryType,
        string cardNumber,
        string accountNumber)
    {
        SalaryType = salaryType;
        CardNumber = cardNumber;
        AccountNumber = accountNumber;
    }

    public FinancialInfoRequestDto()
    {
        
    }
}
