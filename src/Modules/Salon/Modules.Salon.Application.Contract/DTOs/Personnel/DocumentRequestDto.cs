using Modules.Personnel.Domain.Enums;

namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed class DocumentRequestDto
{
    public DocumentType DocumentType { get; init; }
    public string FileName { get; init; }
    public string? Url { get; init; }

    public DocumentRequestDto(
        DocumentType documentType,
        string fileName,
        string? url)
    {
        DocumentType = documentType;
        FileName = fileName;
        Url = url;
    }

    public DocumentRequestDto()
    {
        
    }
}
