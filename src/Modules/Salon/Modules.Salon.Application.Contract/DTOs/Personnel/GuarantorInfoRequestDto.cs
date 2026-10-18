namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed class GuarantorInfoRequestDto
{
    public string GuarantorName { get; init; }
    public string Relationship { get; init; }
    public string GuarantorPhone { get; init; }

    public GuarantorInfoRequestDto(
        string guarantorName,
        string relationship,
        string guarantorPhone)
    {
        GuarantorName = guarantorName;
        Relationship = relationship;
        GuarantorPhone = guarantorPhone;
    }

    public GuarantorInfoRequestDto()
    {
        
    }
}
