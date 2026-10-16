using Modules.Personnel.Domain.Enums;

namespace Modules.Personnel.Domain.Entities;

public sealed class PersonnelDocument
{
    public long Id { get; private set; }
    public DocumentType DocumentType { get; private set; }
    public string FileName { get; private set; } = null!;
    public string? Url { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PersonnelDocument()
    {
    }

    public PersonnelDocument(DocumentType documentType, string fileName, string? url)
    {
        DocumentType = documentType;
        FileName = fileName?.Trim() ?? throw new ArgumentNullException(nameof(fileName));
        Url = string.IsNullOrWhiteSpace(url) ? null : url.Trim();
        CreatedAt = DateTime.UtcNow;
    }
}
