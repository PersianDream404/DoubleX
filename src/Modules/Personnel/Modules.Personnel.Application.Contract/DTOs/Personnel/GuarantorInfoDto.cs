namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record GuarantorInfoDto(
    string GuarantorName,
    string Relationship,
    string GuarantorPhone);
