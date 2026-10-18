using Modules.Personnel.Domain.Enums;

namespace Modules.Personnel.Application.Contract.DTOs.Personnel;

public sealed record DocumentDto(
    DocumentType DocumentType,
    string FileName,
    string? Url);
