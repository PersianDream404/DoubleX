namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record FinancialInfoDto(
    string SalaryType,
    string CardNumber,
    string AccountNumber);
